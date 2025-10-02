# syntax=docker/dockerfile:1.6

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopiera csproj först (case-sensitive!)
COPY authsystem/authsystem.csproj authsystem/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Data/Data.csproj Data/

# Restore web-projektet
RUN dotnet restore authsystem/authsystem.csproj

# Kopiera resten av koden
COPY . .

# Publish (utan AssemblyName override)
RUN dotnet publish authsystem/authsystem.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime-image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "authsystem.dll"]
