FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /web
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION="Release"
WORKDIR /src
COPY web.csproj web/
RUN dotnet restore web/web.csproj
WORKDIR /src/web/
COPY . .
RUN dotnet build web.csproj -c ${BUILD_CONFIGURATION} -o /web/build

FROM build AS publish
ARG BUILD_CONFIGURATION="Release"
RUN dotnet publish web.csproj -c ${BUILD_CONFIGURATION} -o /web/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /web
COPY --from=publish /web/publish .
ENTRYPOINT ["dotnet", "web.dll"]