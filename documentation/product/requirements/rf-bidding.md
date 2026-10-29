# RF — LANCE

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> *Pilar 4. Conecta-se diretamente à [contemplação](rf-contemplation.md). Em alto nível até a especificação detalhada.*

---

## RF-BIDDING-001 — Recomendar estratégia de lance — `Alto nível`
**Ator:** Consultor
**Descrição:** a partir do objetivo do cliente (contemplar rápido × pagar menos), o sistema deve recomendar tipo e valor de lance (livre, fixo, embutido) e mostrar o trade-off.
**Critérios de aceite (direção):**
- [ ] Recomendação conectada à estimativa de contemplação ([RF-CONTEMPLATION-001](rf-contemplation.md)).
- [ ] Explicita o trade-off da escolha (ex.: lance embutido reduz a carta).
**Relacionado:** overview pilar 4; persona Cliente Planejador
