# ADR-0003: Multi-tenancy por discriminador em banco compartilhado

## Status
Aceito

## Contexto
A Apta é B2B e um princípio de produto exige que o dado comercial de um escritório **nunca** seja exposto a outro. A escala projetada é de no máximo cerca de uma centena de escritórios. Não queremos o custo operacional de gerenciar múltiplos bancos ou múltiplas conexões por tenant.

## Decisão
Banco de dados **único e compartilhado**, com isolamento lógico por um **discriminador de tenant** (uma coluna identificadora do escritório em cada registro; o nome exato da coluna é detalhe de implementação). O isolamento é imposto de forma transversal na **camada base de repositórios** e reforçado no banco com **Row-Level Security do PostgreSQL**. Não haverá banco nem schema por tenant.

## Consequências

### Positivas
- Operação simples: uma conexão, um banco, um backup.
- RLS dá uma trava de isolamento no próprio banco, além da imposição na aplicação.

### Negativas
- O isolamento é lógico, não físico: exige rigor para nunca vazar o filtro de tenant (mitigação: forçar no repositório base + RLS + testes de isolamento).
- "Noisy neighbor" é teoricamente possível em escala muito alta (não é o caso na escala projetada).

### Neutras
- Caso algum cliente exija isolamento físico no futuro, um tenant pode ser promovido a banco dedicado sem mudar o modelo lógico.

## Alternativas Consideradas
- **Banco por tenant** ou **schema por tenant**: rejeitados. Além de desnecessários na escala atual e explicitamente indesejados, trazem custos concretos:
  - **Conexões**: cada tenant passa a exigir seu próprio pool; com dezenas de escritórios o número de conexões abertas se multiplica, os pools ficam fragmentados e subutilizados, pressiona-se o limite de conexões do servidor PostgreSQL e passa a ser necessário resolver e trocar a conexão correta a cada requisição conforme o tenant.
  - **Migrações**: cada mudança de schema precisa ser aplicada N vezes, uma por banco/schema; uma falha parcial deixa tenants em versões diferentes (schema drift), o que dificulta rollout e rollback e exige orquestração e monitoramento próprios só para manter todos os bancos consistentes.

## Referências
- `documentation/product/overview.md` (princípio "o dado comercial é do cliente"); ADR-0002
