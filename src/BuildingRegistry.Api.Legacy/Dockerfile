FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1.8

LABEL maintainer "agentschap Informatie Vlaanderen <informatie.vlaanderen@vlaanderen.be>"

COPY / /app
WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

EXPOSE 6002/tcp
ENV ASPNETCORE_URLS http://*:6002

ENTRYPOINT ["./init.sh"]
