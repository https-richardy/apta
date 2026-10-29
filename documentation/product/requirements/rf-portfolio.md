# RF — CARTEIRA E PÓS-VENDA

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> *Pilar 6. Transforma a Apta de "ferramenta de simulação" em sistema operacional do escritório.*

---

## RF-PORTFOLIO-001 — Formalizar a proposta fechada — `MVP`
**Ator:** Back-office (ou Consultor)
**Descrição:** ao fechar uma venda, o sistema deve registrar a cota/adesão a partir da proposta, trazendo a carteira para dentro da plataforma.
**Critérios de aceite:**
- [ ] Cota criada vinculada à proposta/simulação de origem e ao cliente.
- [ ] Sem planilha paralela: o registro é a fonte de verdade da cota.
**Relacionado:** persona Back-office (Carla); [RF-SIMULATIONS-006](rf-simulations.md)

## RF-PORTFOLIO-002 — Acompanhar o ciclo de vida da cota — `MVP`
**Ator:** Back-office
**Descrição:** o sistema deve acompanhar parcelas, status de contemplação e uso da carta de crédito de cada cota.
**Critérios de aceite:**
- [ ] Registrar/consultar parcelas (em dia, a vencer, atrasada).
- [ ] Registrar status de contemplação (por sorteio ou lance) e uso da carta.
**Relacionado:** glossário (Contemplação, Carta de crédito, Parcela)

## RF-PORTFOLIO-003 — Alertar proativamente — `MVP`
**Ator:** sistema → Back-office
**Descrição:** o sistema deve sinalizar antecipadamente eventos de risco/oportunidade: parcela a vencer ou atrasar, cota próxima de contemplar.
**Critérios de aceite:**
- [ ] Parcela a vencer/atrasada aparece em destaque antes de virar problema.
- [ ] Pós-venda deixa de ser reativo (não depende de o cliente reclamar).
**Relacionado:** persona Back-office; redução de inadimplência/churn

## RF-PORTFOLIO-004 — Visão de carteira e produtividade — `MVP`
**Ator:** Gestor
**Descrição:** o gestor deve ver a saúde da carteira do escritório (cotas ativas, inadimplência) e a produtividade por consultor, sem pedir relatório manual.
**Critérios de aceite:**
- [ ] Painel com cotas ativas, inadimplência e pipeline do escritório.
- [ ] Recorte por consultor (quem está vendendo, o que está parado).
**Relacionado:** persona Gestor; North Star (conversão)
