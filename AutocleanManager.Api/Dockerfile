FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AutocleanManager.Api/AutocleanManager.Api.csproj AutocleanManager.Api/
RUN dotnet restore AutocleanManager.Api/AutocleanManager.Api.csproj

COPY . .
RUN dotnet publish AutocleanManager.Api/AutocleanManager.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .
# instalar utilitários mínimos para health-check (netcat, bash)
RUN apt-get update \
	&& apt-get install -y --no-install-recommends netcat-openbsd bash \
	&& rm -rf /var/lib/apt/lists/*

# Copiar entrypoint que aguarda o Postgres
COPY AutocleanManager.Api/entrypoint.sh ./entrypoint.sh
RUN chmod +x ./entrypoint.sh

ENTRYPOINT ["/bin/bash", "./entrypoint.sh"]