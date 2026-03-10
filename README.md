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
* `SEED_DEMO_DATA` - (optional) set this to true to fill the database with some sample data once the app is started for the first time. Set to false by default. The data is not seeded if the app is not in the development configuration.

### Deployment
* `SERVER_DOMAIN` - application's domain name (i.e. example.com). Defaults to localhost for local deployment.
* `WEB_MODULE_ADDRESS` - the internal address of the web server. Used to redirect HTTPS traffic to it. Defaults to web:8080 which is the docker container name.
* `EMAILSETTINGS__SMTPHOST` - the SMTP host to be used, i.e. the adress of the outgoing email's server (such as `smtp.gmail.com`)
* `EMAILSETTINGS__SMTPPORT` - the port of the email client.
* `EMAILSETTINGS__SENDEREMAIL` - the email from which emails are to be sent by the server.
* `EMAILSETTINGS__SENDERNAME` - the name which identifies the email server.
* `EMAILSETTINGS__USERNAME` - the username of the email from which emails are to be sent by the server.
* `EMAILSETTINGS__PASSWORD` - the password of the email from which emails are to be sent by the server. Depending on the email provider, this might have to be a specially-generated password ("app password" on gmail).
* `EMAILSETTINGS__USESSL` - whether SSL is to be used when sending emails. Leave it as `true`.