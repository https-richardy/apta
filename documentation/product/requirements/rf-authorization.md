# RF — AUTORIZAÇÃO

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.

---

## RF-AUTHORIZATION-001 — Autorizar ação por permissão — `MVP`
**Ator:** sistema
**Descrição:** cada ação sensível deve exigir a permissão correspondente no formato `apta:recurso:ação` (ex.: `apta:offers:view`).
**Critérios de aceite:**
- [ ] Ação sem a permissão exigida é negada com erro padronizado ([ADR-0008](../../adr/0008-api-contract.md)).
- [ ] A permissão autoriza *o que* fazer; o escopo de tenant decide *sobre quais dados* ([ADR-0003](../../adr/0003-multi-tenancy.md)).
**Relacionado:** [ADR-0009](../../adr/0009-authorization.md); [rf-user-management.md](rf-user-management.md)
