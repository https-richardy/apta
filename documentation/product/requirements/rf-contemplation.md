# RF — CONTEMPLAÇÃO

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> *Pilar 3 — diferencial central. A **direção** está registrada; a especificação detalhada (modelo, fontes de dados, método) virá quando o pilar entrar em desenvolvimento, pois depende de dados de histórico (ex.: atas de assembleia) e de modelagem própria.*

---

## RF-CONTEMPLATION-001 — Estimar expectativa de contemplação — `Alto nível`
**Ator:** Consultor (via resultado ao cliente)
**Descrição:** o sistema deve estimar, com dados, a expectativa de contemplação de uma cota por **sorteio** e por **lance**, considerando o perfil e a maturidade do grupo (grupo novo × maduro) e o histórico de lance.
**Critérios de aceite (direção):**
- [ ] Responde "quando é razoável esperar ser contemplado?" com uma leitura clara, sem promessa falsa.
- [ ] Distingue expectativa por sorteio e por lance.
**Relacionado:** overview pilar 3 (diferencial central); glossário (Contemplação, Lance, Ata de assembleia)
