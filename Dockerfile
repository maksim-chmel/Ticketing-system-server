# ЭТАП 1: Runtime (образ для запуска)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
# Порт 8080 — стандарт для .NET 8
EXPOSE 8080

# ЭТАП 2: Сборка (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Кэшируем зависимости: сначала только .csproj и restore
COPY ["AdminPanelBack/AdminPanelBack.csproj", "AdminPanelBack/"]
RUN dotnet restore "AdminPanelBack/AdminPanelBack.csproj"

# Копируем остальной код и собираем
COPY . .
WORKDIR "/src/AdminPanelBack"
RUN dotnet build "AdminPanelBack.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ЭТАП 3: Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AdminPanelBack.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ЭТАП 4: Финальный чистый образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminPanelBack.dll"]