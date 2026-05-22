#!/usr/bin/env bash
set -e

# Host e porta padrão (docker-compose usa o serviço 'postgres')
HOST=${DB_HOST:-postgres}
PORT=${DB_PORT:-5432}

echo "Esperando Postgres em $HOST:$PORT..."
while ! nc -z "$HOST" "$PORT"; do
  echo "Postgres indisponivel - aguardando 1s"
  sleep 1
done

echo "Postgres disponivel. Iniciando API..."
exec dotnet AutocleanManager.Api.dll
