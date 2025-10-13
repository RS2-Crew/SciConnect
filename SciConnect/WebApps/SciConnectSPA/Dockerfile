FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build -- --output-hashing=all --aot --optimization

FROM nginx:alpine AS runtime

COPY --from=build /app/dist/sci-connect-spa/browser /usr/share/nginx/html

RUN printf 'server {\n\
  listen 4000;\n\
  server_name _;\n\
  root /usr/share/nginx/html;\n\
  index index.html;\n\
  location / {\n\
    try_files $uri $uri/ /index.html;\n\
  }\n\
}\n' > /etc/nginx/conf.d/default.conf

EXPOSE 4000



CMD ["nginx","-g","daemon off;"]
