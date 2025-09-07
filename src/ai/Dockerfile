FROM pytorch/pytorch:2.8.0-cuda12.9-cudnn9-runtime AS ai
WORKDIR /app
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

# Install dotnet runtime to run the app
RUN apt-get update && apt-get install -y curl libicu-dev git
RUN curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
RUN chmod +x ./dotnet-install.sh
RUN ./dotnet-install.sh --channel 9.0 --runtime aspnetcore --install-dir /usr/share/dotnet
RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
RUN dotnet --info

COPY recognize-anything/ /app/
RUN pip install -r /app/requirements.txt
# overwrite transformers version due to an issue with a missing method
RUN pip install transformers==4.30.2

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ai.csproj ai/
RUN dotnet restore ai/ai.csproj
WORKDIR /src/ai/
COPY . .
RUN dotnet build ai.csproj -c Release -o /app/build
RUN dotnet publish ai.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM ai AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ai.dll"]