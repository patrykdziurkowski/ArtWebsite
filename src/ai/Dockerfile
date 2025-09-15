FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ai.csproj ai/
RUN dotnet restore ai/ai.csproj
WORKDIR /src/ai/
COPY . .
RUN dotnet build ai.csproj -c Release -o /app/build
RUN dotnet publish ai.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM pytorch/pytorch:2.7.1-cuda11.8-cudnn9-runtime AS runtime
WORKDIR /app
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

# Install dotnet runtime to run the app
RUN apt-get update && apt-get install -y curl libicu-dev git && rm -rf /var/lib/apt/lists/*
RUN curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh \
        && chmod +x ./dotnet-install.sh \
        && ./dotnet-install.sh --channel 9.0 --runtime aspnetcore --install-dir /usr/share/dotnet \
        && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
        && rm dotnet-install.sh

COPY recognize-anything/ /app/
RUN pip install --no-cache-dir -r /app/requirements.txt \
        && pip install --no-cache-dir transformers==4.30.2

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ai.dll"]