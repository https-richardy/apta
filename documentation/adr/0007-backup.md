# ADR-0007: Estratégia de backup e recuperação

## Status
Aceito

## Contexto
A disponibilidade alvo é 24/7 e o sistema guarda dados financeiros e pessoais. A perda de dados é inaceitável no longo prazo; precisamos de um baseline simples e previsível de backup já no início, com um caminho de evolução claro.

## Decisão
**Backup completo diário** do PostgreSQL à **meia-noite** como baseline (RPO de até 24 horas). Evolução planejada: habilitar **WAL archiving / point-in-time recovery (PITR)** para reduzir o RPO quando o volume financeiro justificar.

## Consequências

### Positivas
- Baseline simples, barato e previsível.

### Negativas
- RPO de até 24h: no pior caso, perde-se até um dia de propostas/parcelas — aceitável no MVP, arriscado em escala financeira (por isso o PITR na evolução).

### Neutras
- A disponibilidade 24/7 será sustentada por estratégia de deploy e monitoramento, fora do escopo deste ADR.

## Alternativas Consideradas
- **Sem backup / apenas snapshots manuais**: rejeitado.
- **PITR desde já**: adiado — complexidade operacional desnecessária no MVP.

## Referências
- ADR-0002; NFRs (disponibilidade 24/7, backup diário)
