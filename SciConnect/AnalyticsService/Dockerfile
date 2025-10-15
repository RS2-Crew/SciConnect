FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AnalyticsService/AnalyticsService.csproj", "AnalyticsService/"]
COPY ["Common/EventBus.Messages/EventBus.Messages.csproj", "Common/EventBus.Messages/"]
RUN dotnet restore "./AnalyticsService/AnalyticsService.csproj"
COPY . .
WORKDIR "/src/AnalyticsService"
RUN dotnet build "./AnalyticsService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AnalyticsService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnalyticsService.dll"]
