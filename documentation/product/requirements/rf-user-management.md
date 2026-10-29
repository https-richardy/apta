# RF — GESTÃO DE USUÁRIOS

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.

---

## RF-USER-MANAGEMENT-001 — Convidar e gerenciar usuários do escritório — `MVP`
**Ator:** Gestor do escritório
**Descrição:** o gestor deve poder convidar, editar e desativar os usuários do seu próprio escritório.
**Critérios de aceite:**
- [ ] Convidar usuário por e-mail (ativação em [RF-AUTHENTICATION-002](rf-authentication.md)).
- [ ] Editar dados e desativar usuário (desativar nunca apaga histórico).
- [ ] Um gestor só enxerga e gerencia usuários do próprio escritório.
- [ ] Reativar usuário desativado sem recriar o cadastro.
**Relacionado:** [ADR-0003](../../adr/0003-multi-tenancy.md); persona Gestor (Marcos)

## RF-USER-MANAGEMENT-002 — Atribuir cargo ao usuário — `MVP`
**Ator:** Gestor do escritório
**Descrição:** o gestor deve poder atribuir um cargo (ex.: Consultor, Gestor, Back-office) a um usuário; o usuário herda as permissões do cargo.
**Critérios de aceite:**
- [ ] Atribuir/remover cargo reflete nas permissões efetivas do usuário.
- [ ] Cargo é um conjunto de permissões (`apta:recurso:ação`), administrável sem deploy.
**Relacionado:** [ADR-0009](../../adr/0009-authorization.md); [rf-authorization.md](rf-authorization.md)

## RF-USER-MANAGEMENT-003 — Respeitar o limite de assentos do plano — `MVP`
**Ator:** sistema
**Descrição:** o número de usuários ativos não pode exceder os assentos contratados na assinatura.
**Critérios de aceite:**
- [ ] Convidar/ativar usuário além dos assentos é bloqueado com erro claro, orientando a ajustar o plano.
- [ ] O consumo de assentos (usados/contratados) fica visível ao gestor.
**Relacionado:** [rf-subscription.md](rf-subscription.md)
