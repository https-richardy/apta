# RECURSOS DO DESENVOLVEDOR

Recursos de desenvolvedor para subir a codebase do zero, rápido.

Esta pasta centraliza os artefatos necessários para reconstruir o **estado
inicial** do ambiente local: export/backup do banco, export do _realm_ do
Keycloak e demais configurações iniciais. A ideia é que, a partir de um
repositório limpo, qualquer pessoa (ou agente) chegue a um ambiente funcional
sem reconstruir nada na mão.

A infraestrutura local é provisionada pelo [`compose.yml`](../compose.yml)
(`apta.infrastructure.postgres`, `apta.infrastructure.keycloak`,
`apta.infrastructure.seq`); os recursos aqui são os **dados e configurações** que
alimentam esses serviços.

> A estrutura é um ponto de partida. Ajuste conforme novos recursos entrarem.

## COMO APLICAR

Suba a infraestrutura antes de restaurar os recursos:

```bash
docker compose up -d
```

### BANCO (`database.bak`)

Restaure o backup no serviço `apta.infrastructure.postgres`. Exemplo (ajuste
nome do banco e usuário conforme seu `.env`):

```bash
# copia o backup para dentro do container
docker compose cp resources/database.bak apta.infrastructure.postgres:/tmp/database.bak

# restaura (pg_restore para dump custom, ou psql para dump .sql)
docker compose exec apta.infrastructure.postgres \
  pg_restore --clean --if-exists -U "$POSTGRES_USER" -d "$POSTGRES_DB" /tmp/database.bak
```
