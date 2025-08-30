FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
EXPOSE 8080
WORKDIR /web

COPY web.csproj .
RUN dotnet restore web.csproj
COPY . .

COPY ./Features/Tags/ImageRecognition/recognize-anything/ /web/image-recognition/
RUN apt-get update && apt-get install -y python3 python3-pip
RUN pip install -r /web/image-recognition/recognize-anything/requirements.txt

ENTRYPOINT [ "dotnet", "watch", "run", "--project", "web.csproj", "--urls", "http://*:8080" ]