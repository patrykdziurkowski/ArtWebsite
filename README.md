# ArtWebsite

## Deploy
1. Clone the repository.
2. Install Docker and Docker Compose.
3. (Optional) Apply migrations to the database by running `docker compose up db-migrations --build`.
4. Copy `.env.default`, rename it to `.env`, and fill out the required environment variables as seen below.
5. Run `docker compose up --build`.

## Run tests
Unlike in the deployment scenario, tests are run on the host's machine and require extra setup. This will vary between platforms but you will generally need to do things described below.
1. Clone the repository.
2. Install Docker, Docker Compose, and Docker Buildx.
3. Install .NET 9 SDK (including the runtime), as well as ASP.NET 9 runtime.
4. (Linux) Add the current user to the docker group, and start the docker service.
5. Copy `.env.default`, rename it to `.env`, and fill out the required environment variables as seen below.
6. Execute tests (either from CLI via `dotnet test` or from your IDE of choice).

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