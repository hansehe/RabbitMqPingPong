FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY . .
WORKDIR /src/RabbitMqPingPong/Services/RabbitMqPingPong.Service
RUN dotnet publish RabbitMqPingPong.Service.csproj -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS final

ENV TZ=Europe/Oslo
ENV RUNNING_IN_CONTAINER=true

HEALTHCHECK --interval=30s --timeout=10s --retries=3 --start-period=10s CMD curl --silent --fail localhost:80/status/health || exit 1

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "RabbitMqPingPong.Service.dll"]
