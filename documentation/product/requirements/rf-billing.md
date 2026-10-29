# RF — COBRANÇA

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> Execução financeira da [assinatura](rf-subscription.md) via provedor de pagamento (ex.: Stripe). A escolha do provedor é uma decisão de arquitetura (ADR a registrar).

---

## RF-BILLING-001 — Processar pagamento via provedor externo — `MVP`
**Ator:** Gestor / sistema
**Descrição:** a cobrança deve ser processada por um provedor de pagamento (ex.: Stripe), sem a Apta armazenar dados de cartão.
**Critérios de aceite:**
- [ ] Dados de cartão ficam no provedor (conformidade PCI), nunca na base da Apta.
- [ ] O gestor pode adicionar e trocar o meio de pagamento.
- [ ] O estado de pagamento é sincronizado a partir do provedor (ex.: webhooks).
**Relacionado:** [ADR-0006](../../adr/0006-data-protection-lgpd.md) (não guardar dado sensível); ADR de provedor de pagamento (a criar)

## RF-BILLING-002 — Cobrança recorrente mensal — `MVP`
**Ator:** sistema
**Descrição:** o sistema deve cobrar automaticamente o valor do plano a cada ciclo mensal.
**Critérios de aceite:**
- [ ] Cobrança automática na data de renovação.
- [ ] O valor reflete os assentos e módulos vigentes ([rf-subscription.md](rf-subscription.md)).
**Relacionado:** [rf-subscription.md](rf-subscription.md)

## RF-BILLING-003 — Tratar falha de pagamento — `MVP`
**Ator:** sistema
**Descrição:** em falha de cobrança, o sistema deve notificar, repetir conforme política e, persistindo, suspender o acesso.
**Critérios de aceite:**
- [ ] Notifica o gestor na falha e a cada nova tentativa.
- [ ] Há período de carência antes de suspender por inadimplência.
- [ ] Suspensão por inadimplência retém os dados conforme [ADR-0006](../../adr/0006-data-protection-lgpd.md).
**Relacionado:** [rf-subscription.md](rf-subscription.md)

## RF-BILLING-004 — Histórico e comprovantes de cobrança — `MVP`
**Ator:** Gestor
**Descrição:** o gestor deve acessar o histórico de cobranças e o comprovante de cada pagamento.
**Critérios de aceite:**
- [ ] Histórico de cobranças consultável (data, valor, status).
- [ ] Comprovante/recibo disponível por cobrança.
**Relacionado:** [rf-subscription.md](rf-subscription.md)
