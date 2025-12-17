# ArtWebsite
## Setup
1. (Optional) Apply migrations to the database by running `docker compose up db-migrations --build`.
2. Set the required environment variables in a `.env` file in project root.
3. Run `docker compose up --build`.
## Environment variables
### Local development
* `MSSQL_SA_PASSWORD` - the password used for SQL Server's admin account.
* `SELENIUM_HEADLESS` - (optional) set this to false to have the browser window open when running Selenium tests. Useful for debugging.
* `ROOT_USERNAME` - the username for the initial administrator account.
* `ROOT_EMAIL` - the email for the initial administrator account.
* `ROOT_PASSWORD` - the password for the initial administrator account.
### Deployment
* `MSSQL_SA_PASSWORD` - the password used for SQL Server's admin account.
* `SERVER_DOMAIN` - application's domain name (i.e. example.com). Defaults to localhost for local deployment.
* `WEB_MODULE_ADDRESS` - the internal address of the web server. Used to redirect HTTPS traffic to it. Defaults to web:8080 which is the docker container name.
* `ROOT_USERNAME` - the username for the initial administrator account.
* `ROOT_EMAIL` - the email for the initial administrator account.
* `ROOT_PASSWORD` - the password for the initial administrator account.