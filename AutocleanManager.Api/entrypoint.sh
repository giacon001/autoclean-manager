#!/usr/bin/env bash
set -e

# Host e porta padrão (docker-compose usa o serviço 'postgres')
HOST=${DB_HOST:-postgres}
PORT=${DB_PORT:-5432}

# Quando hospedado (Render), o banco vem dentro de DATABASE_URL.
if [ -n "$DATABASE_URL" ]; then
  sem_protocolo=${DATABASE_URL#*://}   # usuario:senha@host:porta/banco
  autoridade=${sem_protocolo#*@}        # host:porta/banco
  host_porta=${autoridade%%/*}          # host:porta
  HOST=${host_porta%%:*}
  if [ "$host_porta" != "$HOST" ]; then
    PORT=${host_porta#*:}
  else
    PORT=5432
  fi
fi

echo "Esperando Postgres em $HOST:$PORT..."
while ! nc -z "$HOST" "$PORT"; do
  echo "Postgres indisponivel - aguardando 1s"
  sleep 1
done

echo "Postgres disponivel. Iniciando API..."
exec dotnet AutocleanManager.Api.dll
