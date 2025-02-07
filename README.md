# ArtWebsite
## Setup
1. (Optional) Apply migrations to the database by running `docker compose up db-migrations --build`.
2. Set the required environment variables in a `.env` file in project root.
3. Run `docker compose up --build`.
## Environment variables
* `MSSQL_SA_PASSWORD` - the password used for SQL Server's admin account.
* `SERVER_DOMAIN` - application's domain name (i.e. example.com). Defaults to localhost for local deployment.
* `WEB_MODULE_ADDRESS` - the internal address of the web server. Used to redirect HTTPS traffic to it. Defaults to web:8080 which is the docker container name.
* `BUILD_CONFIGURATION` - (optional) Debug or Release. Defaults to Release. Mainly affects the option to debug the application when set to Debug.
* `ASPNETCORE_ENVIRONMENT` - (optional) Development or Release. Defauls to Release. Primarily affects the information displayed on the website when in development.