# ArtWebsite
## Setup
1. (Optional) Apply migrations to the database by running `docker compose up db-migrations --build`.
2. Copy `.env.default`, rename it to `.env`, and fill out the required environment variables as seen below.
3. Run `docker compose up --build`.
## Environment variables
### Main
* `MSSQL_SA_PASSWORD` - the password used for SQL Server's admin account.
* `ROOT_USERNAME` - the username for the initial administrator account.
* `ROOT_EMAIL` - the email for the initial administrator account.
* `ROOT_PASSWORD` - the password for the initial administrator account.
* `REVIEW_COOLDOWN_SECONDS` - the time the user has to wait for before being able to review a given art piece. Has a default value of 10.

### Local development
* `SELENIUM_HEADLESS` - (optional) set this to false to have the browser window open when running Selenium tests. Useful for debugging.

### Deployment
* `SERVER_DOMAIN` - application's domain name (i.e. example.com). Defaults to localhost for local deployment.
* `WEB_MODULE_ADDRESS` - the internal address of the web server. Used to redirect HTTPS traffic to it. Defaults to web:8080 which is the docker container name.