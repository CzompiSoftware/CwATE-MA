#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#### Base
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS base
WORKDIR /app
EXPOSE 80

RUN apk add --no-cache icu-libs

ENV \
    # Connection string
    CONNECTION_STRING="" \
    # CWCT-MA version
    CWCTMA_VERSION=4.1.0-build15 \
    # Disable the invariant mode (set in base image)
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    # Colors in log
    ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=false \
    ASPNETCORE_URLS=http://+:80

VOLUME [ "/data" ]

#### Build
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build

RUN apk add --no-cache icu-libs

# Fixes issue mentioned in #11
RUN dotnet tool install dotnet-script --tool-path /usr/bin \
    && echo "return;" > tmpscript.cs \
    && dotnet script tmpscript.cs \
    && rm tmpscript.cs

WORKDIR /src

COPY ["CWCT-MA/CWCT-MA.csproj", "CWCT-MA/"]
COPY ["nuget.config", "CWCT-MA/nuget.config"]
RUN dotnet restore "CWCT-MA/CWCT-MA.csproj"
COPY . .
WORKDIR "/src/CWCT-MA"
RUN dotnet build "CWCT-MA.csproj" -c Release -o /app/build

#### Publish
FROM build AS publish
RUN dotnet publish "CWCT-MA.csproj" -c Release -o /app/publish

#### Final
FROM base AS final

WORKDIR /app

RUN mkdir -p /root/.cache/dotnet-script

COPY --from=publish /app/publish .
COPY --from=publish /root/.cache/dotnet-script /root/.cache/dotnet-script

HEALTHCHECK CMD curl --fail http://localhost || exit 1 # test if it is working. Source: https://www.docker.com/blog/9-tips-for-containerizing-your-net-application/

ENTRYPOINT  ["dotnet", "CWCT-MA.dll"]