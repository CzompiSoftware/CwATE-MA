FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

ENV \
    # CWCT-MA version
    CwctMa_VERSION=4.0.1-build14

VOLUME [ "/data" ]


# Download CWCT-MA binaries
RUN wget https://packages.czompi.cloud/products/czsoft/cwct-ma/$CwctMa_VERSION.tar.gz -O cwct-ma.tar.gz \
    && tar -ozxf cwct-ma.tar.gz -C /bin \
    && rm cwct-ma.tar.gz \
    && wget https://packages.czompi.cloud/products/czsoft/cwct-ma/appsettings.json -O /bin/appsettings.json
WORKDIR /bin
EXPOSE 80
ENTRYPOINT ["dotnet", "CWCT-MA.dll"]