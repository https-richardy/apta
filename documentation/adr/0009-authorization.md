# ADR-0009: Autorização baseada em papéis (RBAC)

## Status
Aceito

## Contexto
O ADR-0004 resolve **identidade** (quem é o usuário, via Keycloak), mas não **autorização** (o que ele pode fazer). Precisamos de um modelo de autorização simples de raciocinar, granular o bastante para o produto B2B multi-tenant (gestor, consultor, back-office) e que se encaixe no Keycloak e no isolamento por tenant (ADR-0003).

## Decisão
Adotar um **RBAC de granularidade fina**, em que **uma role equivale a uma permissão** do sistema:

- **Permissão = role**, nomeada no formato `app:resource:action` — ex.: `apta:offers:view` (ver ofertas). É a unidade atômica de autorização.
- **Cargos agregam permissões.** Um cargo (ex.: Consultor, Gestor, Back-office) é um conjunto de permissões. No Keycloak, **cargos são grupos**, e as permissões (roles) são atribuídas a esses grupos.
- **O usuário herda as permissões do cargo.** Ao entrar em um grupo (cargo) no Keycloak, o usuário passa a ter todas as permissões vinculadas a ele; o token carrega essas permissões.
- **A aplicação valida a permissão exata** da ação (ex.: o endpoint de ver oferta exige `apta:offers:view`), a partir de um **catálogo de permissões** mantido no código — a fonte de verdade do que existe.
- **Autorização e tenancy são ortogonais.** A permissão responde *o que* o usuário pode fazer; o **discriminador de tenant** (ADR-0003) responde *sobre quais dados*. As duas camadas são sempre aplicadas juntas: ter `apta:offers:view` não permite ver a oferta de outro escritório.

## Consequências

### Positivas
- Modelo simples e explícito: autorizar = checar se o usuário tem a permissão `app:resource:action`.
- Granularidade fina sem explosão de condicional no código; cargos podem ser remodelados sem tocar nas verificações.
- Encaixa no Keycloak (grupos + roles) sem lógica de autorização caseira (coerente com o ADR-0004).

### Negativas
- Exige manter o catálogo de permissões e o mapeamento cargo → permissões em dia.
- O número de permissões finas tende a crescer; depende de uma convenção de nomes clara (`app:resource:action`).

### Neutras
- A composição cargo → permissões pode ser administrada no Keycloak, sem necessidade de deploy.

## Alternativas Consideradas
- **Papéis "grossos" (uma role por cargo, ex.: `manager`) com lógica condicional na aplicação**: rejeitado — mistura cargo com permissão, espalha regra pelo código e dificulta a granularidade.
- **ABAC (autorização por atributos/políticas)**: não adotado — poder expressivo além do necessário agora; o cruzamento com tenant já é coberto pelo discriminador (ADR-0003).

## Referências
- Keycloak documentation: https://www.keycloak.org/documentation
- NIST RBAC: https://csrc.nist.gov/projects/role-based-access-control
- ADR-0003; ADR-0004
