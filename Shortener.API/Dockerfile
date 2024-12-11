FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/

COPY ./Shortener.DAL/Shortener.DAL.csproj ./Shortener.DAL/Shortener.DAL.csproj
COPY ./Shortener.BLL/Shortener.BLL.csproj ./Shortener.BLL/Shortener.BLL.csproj
COPY ./Shortener.API/Shortener.API.csproj ./Shortener.API/Shortener.API.csproj
# Відновлення залежностей
RUN dotnet restore ./Shortener.API/Shortener.API.csproj

COPY ./Shortener.DAL/ ./Shortener.DAL/
COPY ./Shortener.BLL/ ./Shortener.BLL/
COPY ./Shortener.API/ ./Shortener.API/

# Публікація
RUN dotnet publish -c Release -o ./app/ ./Shortener.API/ --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app/
COPY --from=build /src/app/ .
ENV ASPNETCORE_HTTP_PORTS=5001

ENTRYPOINT [ "dotnet", "Shortener.API.dll" ]
