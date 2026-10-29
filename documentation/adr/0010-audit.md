# ADR-0010: Trilha de auditoria

## Status
Aceito (diretriz; detalhes de implementação evoluem)

## Contexto
A Apta opera sobre dados **financeiros e sensíveis** (ofertas, tabelas, comissões, propostas) e carrega o princípio de produto "o dado comercial é do cliente". Mudanças nesses dados — em especial **alteração de valores** (comissão, taxa, faixa de carta, condições de uma proposta) — precisam ser rastreáveis: *quem* mudou, *o quê*, *quando* e *de qual valor para qual valor*. Isso é exigido tanto pela natureza financeira quanto pela LGPD (ADR-0006), e os **logs de observabilidade** (Seq, ADR-0005) não servem para isso: são efêmeros, voltados a diagnóstico operacional e não são um registro de negócio confiável e consultável.

## Decisão
Manter uma **trilha de auditoria append-only** (imutável) no PostgreSQL (ADR-0002), separada dos logs de observabilidade.

- **O que registrar:** operações de escrita sobre as entidades sensíveis (ofertas, tabelas, comissões, propostas e afins), com ênfase em **mudança de valores** — guardando **valor anterior → valor novo** dos campos alterados.
- **O que cada entrada contém:** ator (usuário + tenant/escritório), ação (criação/alteração/remoção), entidade e identificador afetados, campos alterados com antes/depois, e **carimbo de tempo**.
- **Imutável:** as entradas não são editáveis nem removíveis pela aplicação (append-only); correção se faz com uma nova entrada, nunca sobrescrevendo.
- **Isolada por tenant:** a trilha segue o isolamento do ADR-0003 (um escritório nunca vê a auditoria de outro).
- **Imposição transversal:** o registro é capturado de forma central (na camada base de persistência e/ou via eventos de domínio), não espalhado manualmente por cada handler.
- **Cuidado com dado sensível:** a própria trilha pode conter PII e valores confidenciais, portanto está sujeita às diretrizes do ADR-0006 (confidencial por padrão, retenção e acesso controlados); segredos/credenciais nunca entram na trilha.

## Consequências

### Positivas
- Rastreabilidade completa de mudanças financeiras — base para investigação, disputas e confiança no B2B.
- Reforça o princípio "o dado é do cliente" e apoia a conformidade (LGPD e eventual exigência regulatória).

### Negativas
- Volume de dados cresce de forma contínua — exige política de retenção e, eventualmente, particionamento/arquivamento.
- Exige disciplina para definir **o que** é auditável e para capturar corretamente o antes/depois.
- A trilha concentra dado sensível, ampliando a superfície a proteger (mitigação: isolamento + diretrizes do ADR-0006).

### Neutras
- Pode evoluir para relatórios/exportação de auditoria por tenant sem alterar esta decisão.
- Uma política de retenção específica da trilha pode virar um ADR próprio no futuro.

## Alternativas Consideradas
- **Usar apenas os logs do Seq (ADR-0005)**: rejeitado — observabilidade é efêmera e operacional, não é um registro de negócio imutável, consultável e retido.
- **Triggers de auditoria no banco**: considerado — robusto, mas dificulta capturar o **ator da aplicação** (usuário/tenant) e acopla a regra ao banco; preferimos capturar na aplicação (podendo complementar com o banco no futuro).
- **Event sourcing completo**: adiado — poder e complexidade além do necessário agora; uma trilha append-only paralela resolve o objetivo sem reescrever o modelo de dados.

## Referências
- `documentation/product/overview.md` (princípio "o dado comercial é do cliente")
- ADR-0002; ADR-0003; ADR-0005; ADR-0006
