# ADR-0004: Identidade e acesso com Keycloak

## Status
Aceito

## Contexto
A Apta precisa de autenticação e autorização para um produto B2B multi-tenant, com usuários por escritório e papéis distintos (gestor, consultor, back-office). Reimplementar toda a lógica de autenticação e autorização do zero é caro, demorado e arriscado em segurança.

## Decisão
Usar **Keycloak** como provedor de identidade (OIDC), delegando a ele login, gestão de usuários, emissão de tokens e a base de autorização. Os escritórios são representados via organizations/groups dentro de um **realm único** (coerente com a estratégia de tenancy do ADR-0003).

## Consequências

### Positivas
- Evita reescrever toda a lógica de autenticação/autorização — ela já existe e é madura no Keycloak.
- OIDC/SSO padrão de mercado; suporta papéis e, futuramente, federação de identidade.
- **Segurança mantida pelo próprio Keycloak**: por ser um projeto maduro e ativo, recebe correção de CVEs e melhorias de segurança continuamente. A responsabilidade de acompanhar e remediar vulnerabilidades do fluxo de identidade sai do time — é exatamente o tipo de encargo que não queremos assumir.

### Negativas
- Adiciona uma dependência operacional de infraestrutura (já prevista).
- Curva de aprendizado para administrar o Keycloak.

### Neutras
- Realm único com organizations/groups alinha-se à tenancy compartilhada (ADR-0003).

## Alternativas Consideradas
- **Autenticação/autorização caseira**: rejeitada — além do custo de desenvolvimento, colocaria sobre o time a responsabilidade permanente de segurança (acompanhar CVEs, aplicar correções, auditar o fluxo de identidade), um encargo contínuo e arriscado que não queremos carregar.
- **Provedor gerenciado (ex.: Auth0)**: não escolhido — Keycloak é open-source e auto-hospedável, sem custo por usuário.

## Referências
- Documentação do Keycloak: https://www.keycloak.org/documentation ; ADR-0003
