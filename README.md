# ArtWebsite
## Setup
1. (Optional) Apply migrations to the database by running `docker compose up db-migrations --build`.
2. Set the required environment variables in a `.env` file in project root.
3. Run `docker compose up --build`.
## Required environment variables
* `MSSQL_SA_PASSWORD` - the password used for SQL Server's admin account.