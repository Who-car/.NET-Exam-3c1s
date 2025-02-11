# Этап сборки: используем официальный образ SDK для компиляции и публикации приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["Backend.WebAPI/Backend.WebAPI.csproj", "Backend.WebAPI/"]
COPY ["Backend.Application/Backend.Application.csproj", "Backend.Application/"]
COPY ["Backend.Domain/Backend.Domain.csproj", "Backend.Domain/"]
COPY ["Backend.Data/Backend.Data.csproj", "Backend.Data/"]

RUN dotnet restore "Backend.WebAPI/Backend.WebAPI.csproj" --verbosity normal

# Копируем остальные файлы и собираем приложение
COPY . .
WORKDIR "/src/Backend.WebAPI"
RUN dotnet publish "Backend.WebAPI.csproj" -c Release -o /app/publish

# Этап выполнения: используем минимальный образ ASP.NET Core Runtime для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем опубликованное приложение из этапа сборки
COPY --from=build /app/publish .

# Если приложение слушает на определённом порту, его можно указать (например, 80)
EXPOSE 8080
EXPOSE 8081

# Точка входа: запускаем приложение
ENTRYPOINT ["dotnet", "Backend.WebAPI.dll"]
