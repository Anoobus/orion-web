FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["orion.web/orion.web.csproj", "orion.web/"]
RUN dotnet restore "orion.web/orion.web.csproj"
COPY . .
WORKDIR "/src/orion.web"
RUN dotnet build "orion.web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "orion.web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENV ASPNETCORE_ENVIRONMENT="Azure"
ENV ASPNETCORE_URLS="https://+;http://+"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/app/orion-live.eastus.azurecr.io.pfx"

ENTRYPOINT ["dotnet", "orion.web.dll"]