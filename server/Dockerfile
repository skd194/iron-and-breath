# Multi-stage build for the ASP.NET Core API.
# Build context is ./server so all projects are available for restore/publish.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first for layer-cached restore.
COPY IronAndBreath.Domain/IronAndBreath.Domain.csproj IronAndBreath.Domain/
COPY IronAndBreath.Infrastructure/IronAndBreath.Infrastructure.csproj IronAndBreath.Infrastructure/
COPY IronAndBreath.Api/IronAndBreath.Api.csproj IronAndBreath.Api/
RUN dotnet restore IronAndBreath.Api/IronAndBreath.Api.csproj

# Copy the rest and publish.
COPY . .
RUN dotnet publish IronAndBreath.Api/IronAndBreath.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# SQLite database lives here; mount a named volume to persist it.
RUN mkdir -p /app/data
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "IronAndBreath.Api.dll"]
