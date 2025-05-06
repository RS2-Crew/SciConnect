# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/DB/DB.API/DB.API/DB.API.csproj", "Services/DB/DB.API/DB.API/"]
COPY ["DB.Infrastructure/DB.Infrastructure.csproj", "DB.Infrastructure/"]
COPY ["Services/DB/DB.Application/DB.Application.csproj", "Services/DB/DB.Application/"]
COPY ["Services/DB/DB.Domain/DB.Domain.csproj", "Services/DB/DB.Domain/"]
RUN dotnet restore "./Services/DB/DB.API/DB.API/DB.API.csproj"
COPY . .
WORKDIR "/src/Services/DB/DB.API/DB.API"
RUN dotnet build "./DB.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DB.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DB.API.dll"]