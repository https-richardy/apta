# RF — ASSINATURA

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> O modelo é assinatura SaaS B2B por escritório, com faixas por assentos (consultores) e módulos. A cobrança em si vive em [rf-billing.md](rf-billing.md).

---

## RF-SUBSCRIPTION-001 — Assinar plano mensal — `MVP`
**Ator:** Gestor
**Descrição:** o escritório deve poder assinar um plano mensal pago, convertendo o trial em assinatura ativa sem perder dados.
**Critérios de aceite:**
- [ ] Escolher plano, número de assentos (consultores) e módulos contratados.
- [ ] Ao assinar, define-se o ciclo de cobrança mensal e a próxima data de renovação.
- [ ] O trial converte em assinatura ativa preservando catálogo, carteira e usuários.
**Relacionado:** overview (modelo de receita); [rf-billing.md](rf-billing.md)

## RF-SUBSCRIPTION-002 — Ajustar assentos e módulos — `MVP`
**Ator:** Gestor
**Descrição:** o gestor deve poder mudar o número de assentos e os módulos contratados, refletindo na cobrança.
**Critérios de aceite:**
- [ ] Aumentar/reduzir assentos e ativar/desativar módulos ajusta o valor do próximo ciclo.
- [ ] Reduzir assentos abaixo dos usuários ativos é bloqueado até desativar usuários ([RF-USER-MANAGEMENT-003](rf-user-management.md)).
**Relacionado:** [rf-user-management.md](rf-user-management.md); overview (faixas por assentos/módulos)

## RF-SUBSCRIPTION-003 — Cancelar assinatura — `MVP`
**Ator:** Gestor
**Descrição:** o gestor deve poder cancelar a assinatura a qualquer momento, de forma self-service, sem barreira abusiva.
**Critérios de aceite:**
- [ ] Cancelamento é self-service (sem precisar falar com a Apta).
- [ ] O acesso é mantido até o fim do período já pago; depois, a conta é suspensa.
- [ ] Conta suspensa retém os dados conforme a política de retenção ([ADR-0006](../../adr/0006-data-protection-lgpd.md)).
**Relacionado:** [rf-billing.md](rf-billing.md)

## RF-SUBSCRIPTION-004 — Reativar assinatura — `MVP`
**Ator:** Gestor
**Descrição:** um escritório suspenso ou cancelado deve poder reativar a assinatura e recuperar o acesso aos dados ainda retidos.
**Critérios de aceite:**
- [ ] Reativar restaura o acesso aos dados dentro do período de retenção.
- [ ] Após o período de retenção, a reativação não garante a recuperação dos dados (conforme a política).
**Relacionado:** [ADR-0006](../../adr/0006-data-protection-lgpd.md); [rf-billing.md](rf-billing.md)

## RF-SUBSCRIPTION-005 — Acompanhar status da assinatura — `MVP`
**Ator:** Gestor
**Descrição:** o gestor deve ver claramente o estado da assinatura e as datas-chave.
**Critérios de aceite:**
- [ ] Mostra o status (trial, ativa, inadimplente, cancelada, suspensa) e a próxima data de cobrança.
- [ ] Avisa antes do fim do trial e antes da renovação.
**Relacionado:** [rf-onboarding.md](rf-onboarding.md); [rf-billing.md](rf-billing.md)
