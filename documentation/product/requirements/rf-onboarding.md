# RF — CADASTRO E ONBOARDING

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> Como um novo escritório entra na Apta: cadastro self-service, criação da empresa (tenant) e início do teste gratuito.

---

## RF-ONBOARDING-001 — Criar conta (self-service) — `MVP`
**Ator:** novo cliente (futuro Gestor)
**Descrição:** um novo cliente deve poder criar sua conta sozinho, sem intervenção manual da Apta, tornando-se o primeiro gestor do escritório.
**Critérios de aceite:**
- [ ] Cadastro self-service com verificação de e-mail (ativação em [RF-AUTHENTICATION-002](rf-authentication.md)).
- [ ] O primeiro usuário do escritório recebe o cargo de Gestor.
- [ ] Não é possível concluir o cadastro sem aceitar os termos de uso.
**Relacionado:** [ADR-0004](../../adr/0004-identity.md); persona Gestor (Marcos)

## RF-ONBOARDING-002 — Cadastrar a empresa (escritório) — `MVP`
**Ator:** Gestor
**Descrição:** o gestor deve cadastrar sua empresa (escritório/representante), o que provisiona o espaço isolado (tenant) do escritório.
**Critérios de aceite:**
- [ ] Coletar dados da empresa (razão social, nome fantasia, CNPJ) com validação de CNPJ.
- [ ] A criação do escritório provisiona o tenant isolado ([ADR-0003](../../adr/0003-multi-tenancy.md)).
- [ ] Os dados do escritório nunca se cruzam com os de outro escritório.
**Relacionado:** glossário (Representante / Ponto de venda)

## RF-ONBOARDING-003 — Iniciar teste gratuito de 1 mês — `MVP`
**Ator:** Gestor
**Descrição:** ao concluir o cadastro da empresa, o escritório deve iniciar automaticamente um teste gratuito de 30 dias, com acesso ao produto, sem exigir pagamento para começar.
**Critérios de aceite:**
- [ ] O trial de 1 mês começa no cadastro, sem cartão obrigatório para iniciar.
- [ ] O sistema mostra, de forma visível, quantos dias restam do trial.
- [ ] O produto fica utilizável durante o trial (conforme o plano de avaliação).
- [ ] Ao fim do trial sem assinatura, o acesso é bloqueado, mas os dados são retidos ([RF-SUBSCRIPTION-003](rf-subscription.md)).
**Relacionado:** [rf-subscription.md](rf-subscription.md)

## RF-ONBOARDING-004 — Primeiros passos pós-cadastro — `MVP`
**Ator:** Gestor
**Descrição:** logo após entrar, o gestor deve ser guiado a extrair valor rápido: importar a primeira tabela de ofertas e convidar seus consultores.
**Critérios de aceite:**
- [ ] Um guia/checklist inicial aponta para importar tabela ([RF-OFFER-CATALOG-001](rf-offer-catalog.md)) e convidar usuários ([RF-USER-MANAGEMENT-001](rf-user-management.md)).
- [ ] O guia pode ser dispensado e retomado.
**Relacionado:** persona Gestor; North Star (adoção)
