using System.IdentityModel.Tokens.Jwt;
using System.Text;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Shortener.BLL.Interfaces;
using Shortener.BLL.Models;
using Shortener.BLL.Services;
using Shortener.BLL.Validators;
using Shortener.DAL.Data;
using Shortener.DAL.Entities;

using WebApp.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);

// HACK: This is a workaround for the issue with the JwtBearerDefaults.AuthenticationScheme
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUrlService, UrlService>();
builder.Services.AddScoped<DbInitializer>();

builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();

builder.Services.AddDbContext<ShortenerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // TODO: add interceptor for setting CreatedAt and UpdatedAt
});

builder.Services.AddIdentity<ShortenerUser, IdentityRole<Guid>>(
        options => options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<ShortenerDbContext>()
    .AddDefaultTokenProviders();

// Спочатку реєструємо JwtSettings як конфігураційний об'єкт
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings is not configured");

// Потім додаємо аутентифікацію
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtSettings.ValidIssuer,
        ValidAudience = jwtSettings.ValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = "roles",
    };
});

// Add Authorization services
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserAndAbove", policy => policy.RequireRole("User", "Admin"));

// Додати CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Використати CORS
app.UseCors("AllowAll");

try
{
    using var scope = app.Services.CreateScope();

    // Ініціалізація бази даних
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.SeedAdminAsync();
    await dbInitializer.SeedUserRoleAsync();
}
catch (InvalidOperationException e)
{
    app.Logger.LogError(e, "Failed to seed the database: {ExceptionMessage}", e.Message);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Migrating database...");
    var db = scope.ServiceProvider.GetRequiredService<ShortenerDbContext>();
    db.Database.Migrate();
}

app.Run();