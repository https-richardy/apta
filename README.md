# APTA

Plataforma B2B para o mercado de consórcio, voltada a simulações e às
operações relacionadas.

> Projeto **privado**. Uso, cópia e distribuição são proibidos sem
> autorização. Veja a [LICENÇA](LICENSE).

---

## PRÉ-REQUISITOS

O que você precisa ter instalado para começar:

| Ferramenta | Para quê | Download |
|------------|----------|----------|
| **Docker Desktop** | rodar a infra local (MongoDB, Keycloak, Seq) | https://www.docker.com/products/docker-desktop/ |
| **.NET 10 SDK** | compilar e rodar o backend | https://dotnet.microsoft.com/download/dotnet/10.0 |
| **DBeaver Community** *(opcional)* | GUI de banco de dados para navegar e gerenciar o PostgreSQL | https://dbeaver.io/ |
| **IDE** | escrever, depurar e executar a codebase | VS Code · https://code.visualstudio.com/ ou Visual Studio · https://visualstudio.microsoft.com/ |

> **Recomendado:** **Visual Studio Community Edition** (gratuito) para a
> experiência completa de .NET. O VS Code funciona bem com a extensão de C#.

---

## RODANDO A INFRAESTRUTURA

A infra local (MongoDB, Keycloak e Seq) sobe via Docker Compose. Ela lê um
arquivo `.env`; então, a partir da raiz do repositório, suba tudo:

```bash
docker compose up -d
```

Serviços disponíveis:

| Serviço | Endereço | Credenciais padrão (`.env`) | Documentação |
|---------|----------|------------------------------|--------------|
| **MongoDB** | `localhost:27017` | `admin` / `admin` | https://www.mongodb.com/docs/manual/ |
| **Keycloak** | http://localhost:8080 | `admin` / `admin` | https://www.keycloak.org/documentation |
| **Seq** (logs) | http://localhost:5341 | `admin` / `admin` | https://docs.datalust.co/docs |

---

## DOCUMENTAÇÃO

A documentação de produto está sendo reescrita para o novo produto e ficará
em [documentation/](documentation/).

---

## LICENÇA

Software proprietário. © 2026 RICHARD HERBERT GARCIA BELGA - ME
(CNPJ 61.243.589/0001-43). Todos os direitos reservados. Veja a
[LICENÇA](LICENSE).
