using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Shortener.DAL.Entities;

namespace Shortener.DAL.Data
{
    public class ShortenerDbContext : IdentityDbContext<ShortenerUser, IdentityRole<Guid>, Guid>
    {
        public ShortenerDbContext(DbContextOptions<ShortenerDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortenerUser> ShortenerUsers { get; set; } = default!;

        public DbSet<ShortUrl> ShortUrls { get; set; } = default!;

        public DbSet<AboutInfo> AboutInfos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ShortUrl>()
                .HasIndex(static s => s.ShortenedUrl)
                .IsUnique();

            builder.Entity<ShortUrl>()
                .HasOne(static s => s.CreatedBy)
                .WithMany(static u => u!.ShortUrls)
                .HasForeignKey(static s => s.CreatedById)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }
}