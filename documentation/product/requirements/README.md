# APTA — REQUISITOS FUNCIONAIS (MVP)

> Documento de produto. Lista **o que** o sistema deve fazer, de forma rastreável. Não trata de implementação (o *como* vive nos [ADRs](../../adr/README.md)). A linguagem segue o [glossário](../glossary.md); os papéis seguem as [personas](../personas.md); as capacidades seguem os pilares do [overview](../overview.md).

Cada área de requisito tem seu próprio arquivo, com **todos** os seus RFs.

---

## CONVENÇÕES

- Cada requisito tem um identificador **`RF-<ÁREA>-NNN`** por extenso (ex.: `RF-AUTHENTICATION-001`), estável — não reaproveitado se um RF for removido.
- Cada RF traz **Ator**, **Descrição**, **Critérios de aceite** (checklist) e **Relacionado** (pilar / ADR / persona).
- **Status:** `MVP` (entra na primeira versão) · `Alto nível` (direção registrada, especificação ainda a detalhar).
- **Valem para todos os RFs** (não repetimos em cada item):
  - Isolamento por escritório (tenant) — [ADR-0003](../../adr/0003-multi-tenancy.md).
  - Auditoria de mudanças em dados sensíveis — [ADR-0010](../../adr/0010-audit.md).
  - Contrato de API e erros padronizados — [ADR-0008](../../adr/0008-api-contract.md).

---

## ÍNDICE

### Plataforma (SaaS)

| Área | Arquivo | RFs |
| :--- | :--- | :--- |
| Cadastro e onboarding | [rf-onboarding.md](rf-onboarding.md) | `RF-ONBOARDING-001..004` |
| Autenticação | [rf-authentication.md](rf-authentication.md) | `RF-AUTHENTICATION-001..004` |
| Gestão de usuários | [rf-user-management.md](rf-user-management.md) | `RF-USER-MANAGEMENT-001..003` |
| Autorização | [rf-authorization.md](rf-authorization.md) | `RF-AUTHORIZATION-001` |
| Assinatura | [rf-subscription.md](rf-subscription.md) | `RF-SUBSCRIPTION-001..005` |
| Cobrança | [rf-billing.md](rf-billing.md) | `RF-BILLING-001..004` |

### Produto (pilares de consórcio)

| Área | Arquivo | RFs | Pilar |
| :--- | :--- | :--- | :--- |
| Catálogo de ofertas | [rf-offer-catalog.md](rf-offer-catalog.md) | `RF-OFFER-CATALOG-001..006` | 1 |
| Simulação | [rf-simulations.md](rf-simulations.md) | `RF-SIMULATIONS-001..006` | 2 |
| Carteira e pós-venda | [rf-portfolio.md](rf-portfolio.md) | `RF-PORTFOLIO-001..004` | 6 |
| Contemplação | [rf-contemplation.md](rf-contemplation.md) | `RF-CONTEMPLATION-001` | 3 |
| Lance | [rf-bidding.md](rf-bidding.md) | `RF-BIDDING-001` | 4 |
| Comparador | [rf-comparator.md](rf-comparator.md) | `RF-COMPARATOR-001` | 5 |

> Contemplação, Lance e Comparador estão em **Alto nível**: dependem de modelagem e de dados de histórico (ex.: atas de assembleia) ainda a especificar.
