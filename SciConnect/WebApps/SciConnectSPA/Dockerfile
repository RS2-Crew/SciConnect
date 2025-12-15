FROM node:20-alpine AS build
WORKDIR /app

COPY WebApps/SciConnectSPA/package*.json ./
RUN npm ci

COPY WebApps/SciConnectSPA/ .
RUN npm run build -- --output-hashing=all --aot --optimization

FROM nginx:alpine AS runtime

COPY --from=build /app/dist/sci-connect-spa/browser /usr/share/nginx/html

# Create env-config.js template that will be populated at runtime
RUN echo 'window.__env = window.__env || {};' > /usr/share/nginx/html/env-config.template.js

# Create startup script to inject environment variables
RUN printf '#!/bin/sh\n\
echo "window.__env = {" > /usr/share/nginx/html/env-config.js\n\
echo "  API_DB_URL: \"${API_DB_URL:-http://localhost:4001/api/v1/db}\"," >> /usr/share/nginx/html/env-config.js\n\
echo "  API_IDENTITY_URL: \"${API_IDENTITY_URL:-http://localhost:4000/api/v1/identity}\"," >> /usr/share/nginx/html/env-config.js\n\
echo "  API_ANALYTICS_URL: \"${API_ANALYTICS_URL:-http://localhost:4002/api/v1/analytics}\"" >> /usr/share/nginx/html/env-config.js\n\
echo "};" >> /usr/share/nginx/html/env-config.js\n\
exec nginx -g "daemon off;"\n' > /docker-entrypoint.sh && chmod +x /docker-entrypoint.sh

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

CMD ["/docker-entrypoint.sh"]
