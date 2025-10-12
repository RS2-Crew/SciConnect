FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__AutoDb="Data Source=/app/data/autocomplete.db"
ENV Rabbit__Host="rabbitmq://rabbitmq"
ENV Rabbit__User="guest"
ENV Rabbit__Pass="guest"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Autocomplete.API/Autocomplete.API.csproj", "Autocomplete.API/"]
COPY ["SciConnect/Common/EventBus.Messages/EventBus.Messages.csproj", "Common/EventBus.Messages/"]
RUN dotnet restore "Autocomplete.API/Autocomplete.API.csproj"
COPY . .
WORKDIR "/src/Autocomplete.API"
RUN dotnet build "Autocomplete.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Autocomplete.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/data
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Autocomplete.API.dll"]
