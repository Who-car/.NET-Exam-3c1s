# Stage 1: Сборка React-приложения
FROM node:18-alpine AS build

# Задаём рабочую директорию внутри контейнера
WORKDIR /app

# Копируем файлы package.json и package-lock.json (или yarn.lock)
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем весь исходный код в контейнер
COPY . .

# Собираем приложение для production
RUN npm run build

# Stage 2: Развёртывание с помощью Nginx
FROM nginx:alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf

# Копируем собранное приложение из предыдущего stage в директорию, откуда Nginx отдаёт статику
COPY --from=build /app/dist /usr/share/nginx/html

# Если требуется, можно заменить дефолтный конфиг Nginx
# COPY nginx.conf /etc/nginx/conf.d/default.conf

# Открываем порт 80 для доступа к приложению
EXPOSE 80

# Запускаем Nginx в foreground режиме
CMD ["nginx", "-g", "daemon off;"]