# ADR-0002: PostgreSQL como banco de dados primário

## Status
Aceito

## Contexto
A Apta manipula dados **financeiros** (propostas, parcelas, comissões) que exigem consistência transacional (ACID), além de dados fortemente **relacionais** (escritório → consultor → cliente → proposta; administradora → oferta → tabela). O backend vinha configurado com MongoDB, mas essa escolha não foi deliberada pelo time. Para transações ACID e integridade referencial, o indicado é um banco relacional como o PostgreSQL.

## Decisão
Usar **PostgreSQL** como banco de dados primário. Esta decisão **substitui** o MongoDB adotado inicialmente.

## Consequências

### Positivas
- ACID para operações financeiras e integridade referencial para o modelo relacional.
- Row-Level Security disponível, servindo de base para o isolamento multi-tenant (ver ADR-0003).

### Negativas
- A camada `Infrastructure` atual é orientada a MongoDB e precisará ser refeita para PostgreSQL.
- Exige disciplina de modelagem e de índices.

### Neutras
- Cache (Redis) e/ou extensões analíticas podem ser avaliados no futuro sem alterar esta decisão.

## Alternativas Consideradas
- **MongoDB** (adotado inicialmente): rejeitado — fraco em transações entre documentos e em consultas relacionais, que são o núcleo da carteira e do comissionamento.

## Referências
- `documentation/product/overview.md`; ADR-0003
