FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER app
WORKDIR /ai
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ai.csproj ai/
RUN dotnet restore ai/ai.csproj
WORKDIR /src/ai/
COPY . .
RUN dotnet build ai.csproj -c Release -o /ai/build
RUN dotnet publish ai.csproj -c Release -o /ai/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /ai
COPY --from=build /ai/publish .

HEALTHCHECK --interval=1s --timeout=5s --retries=90 \
        CMD curl -f http://localhost:8081/health || exit 1

ENTRYPOINT ["dotnet", "ai.dll"]