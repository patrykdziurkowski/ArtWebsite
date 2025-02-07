FROM caddy:2.8.4-alpine
EXPOSE 80
EXPOSE 443

# Copy in the custom Caddyfile
ARG SERVER_DOMAIN=localhost
ARG WEB_MODULE_ADDRESS=web:8080
RUN echo -e "${SERVER_DOMAIN} {\n    reverse_proxy ${WEB_MODULE_ADDRESS}\n}" > /etc/caddy/Caddyfile
RUN test -e /etc/caddy/Caddyfile || (echo "Caddyfile was not copied over successfully" && false)