# ADRs - Architecture Decision Records

Registro das decisões de arquitetura da Apta. Cada ADR captura **uma** decisão: o contexto que a motivou, a decisão em si, as consequências (positivas, negativas e neutras) e as alternativas avaliadas.

## Regras

- ADRs são **imutáveis**. Uma decisão que muda não é reescrita: cria-se um novo ADR que **substitui** o anterior (que passa a `Substituído por ADR-XXX`).
- Numeração sequencial de quatro dígitos: `0001`, `0002`, ...
- Um arquivo por decisão, nomeado `NNNN-short-title.md`.
- O formato segue o [`0000-template.md`](0000-template.md).
- ADR é decisão de **arquitetura/técnica** — decisões de produto vivem em [`../product/`](../product/).

## Índice

| ADR | Título | Status |
| :--- | :--- | :--- |
| [0001](0001-architecture.md) | Monólito em camadas (DDD/Clean) | Aceito |
| [0002](0002-database.md) | PostgreSQL como banco de dados primário | Aceito |
| [0003](0003-multi-tenancy.md) | Multi-tenancy por discriminador em banco compartilhado | Aceito |
| [0004](0004-identity.md) | Identidade e acesso (Keycloak) | Aceito |
| [0005](0005-observability.md) | Observabilidade (Seq e Sentry) | Aceito |
| [0006](0006-data-protection-lgpd.md) | Proteção de dados pessoais (LGPD) | Aceito |
| [0007](0007-backup.md) | Estratégia de backup e recuperação | Aceito |
| [0008](0008-api-contract.md) | Contrato e estilo da API REST | Aceito |
| [0009](0009-authorization.md) | Autorização baseada em papéis (RBAC) | Aceito |
| [0010](0010-audit.md) | Trilha de auditoria | Aceito |

> O índice funciona também como backlog de decisões: um ADR "Em debate" ainda não foi escrito, está sendo discutido.
