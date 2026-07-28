# Build the Vite SPA, then serve it with nginx (which also proxies /api).
# Images are fully-qualified so Podman's short-name resolution doesn't prompt.
FROM docker.io/library/node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM docker.io/library/nginx:1.27-alpine AS runtime
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
