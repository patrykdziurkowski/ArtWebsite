FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /web
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY web.csproj web/
RUN dotnet restore web/web.csproj
WORKDIR /src/web/
COPY . .
RUN dotnet build web.csproj -c Release -o /web/build

FROM build AS publish
RUN dotnet publish web.csproj -c Release -o /web/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /web
COPY --from=publish /web/publish .

COPY ./Features/Tags/ImageRecognition/recognize-anything/ /web/image-recognition/
RUN apt-get update && apt-get install -y python3 python3-pip
RUN pip install -r /web/image-recognition/recognize-anything/requirements.txt

ENTRYPOINT ["dotnet", "web.dll"]