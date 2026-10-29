# ADR-0006: Proteção de dados pessoais e conformidade com a LGPD

## Status
Aceito (diretriz; detalhes operacionais evoluem)

## Contexto
A Apta armazenará dados pessoais de clientes finais (CPF, e possivelmente outros) e de usuários dos escritórios. Isso sujeita o produto à **LGPD (Lei 13.709/2018)**. Além disso, o modelo multi-tenant exige que esses dados não vazem entre escritórios.

## Decisão
Tratar dados pessoais como **confidenciais por padrão**, com as seguintes diretrizes desde o MVP:
- Criptografia **em trânsito** (TLS) e **em repouso**.
- **Minimização**: coletar apenas o dado pessoal necessário à operação.
- **Isolamento por tenant** (ADR-0003) aplicado também aos dados pessoais.
- Base para os **direitos do titular** (acesso e exclusão).

Os detalhes operacionais (política de retenção, anonimização, processos e eventual DPO) serão refinados, mas a diretriz vale desde já.

## Consequências

### Positivas
- Reduz risco legal e reforça a marca de confiança/transparência do produto.

### Negativas
- Exige disciplina de modelagem (marcar campos como PII) e processos de retenção/exclusão.

### Neutras
- Pode demandar um ADR específico futuro para retenção e criptografia em nível de campo.

## Alternativas Consideradas
- **Tratar a conformidade depois**: rejeitado — dado pessoal entra desde o MVP, e adaptar conformidade depois é caro e arriscado.

## Referências
- `documentation/product/overview.md` (princípios de confidencialidade); ADR-0003; Lei 13.709/2018 (LGPD)
