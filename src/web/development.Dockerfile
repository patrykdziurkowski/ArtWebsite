FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
EXPOSE 8080
WORKDIR /web

COPY . .
COPY web.csproj .
RUN dotnet restore web.csproj

ENTRYPOINT [ "dotnet", "watch", "run", "--project", "web.csproj", "--urls", "http://*:8080" ]