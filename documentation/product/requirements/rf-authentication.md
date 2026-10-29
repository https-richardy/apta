# RF — AUTENTICAÇÃO

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> O fluxo de identidade é **delegado ao Keycloak** ([ADR-0004](../../adr/0004-identity.md)); os RFs abaixo expressam o que o produto exige desse fluxo.

---

## RF-AUTHENTICATION-001 — Autenticar usuário — `MVP`
**Ator:** qualquer usuário (consultor, gestor, back-office)
**Descrição:** o sistema deve autenticar o usuário via Keycloak (OIDC) antes de liberar qualquer funcionalidade, identificando a que escritório (tenant) ele pertence.
**Critérios de aceite:**
- [ ] Login e logout via provedor de identidade (sem senha gerida pela aplicação).
- [ ] O token carrega escritório (tenant) e permissões efetivas do usuário.
- [ ] Sessão expira e exige reautenticação conforme política do Keycloak.
- [ ] Usuário desativado ou de escritório suspenso não consegue autenticar.
**Relacionado:** [ADR-0004](../../adr/0004-identity.md); [rf-authorization.md](rf-authorization.md)

## RF-AUTHENTICATION-002 — Ativar conta e definir senha — `MVP`
**Ator:** usuário recém-convidado ou recém-cadastrado
**Descrição:** no primeiro acesso, o usuário deve verificar o e-mail e definir sua senha para ativar a conta.
**Critérios de aceite:**
- [ ] Ativação por link enviado ao e-mail (convite ou auto-cadastro).
- [ ] E-mail precisa ser verificado antes do primeiro login.
- [ ] Senha segue a política de força definida no Keycloak.
**Relacionado:** [rf-user-management.md](rf-user-management.md); [rf-onboarding.md](rf-onboarding.md)

## RF-AUTHENTICATION-003 — Recuperar senha — `MVP`
**Ator:** qualquer usuário
**Descrição:** o usuário deve poder redefinir a própria senha de forma self-service, sem acionar a Apta.
**Critérios de aceite:**
- [ ] Fluxo "esqueci minha senha" por e-mail, com link de expiração curta.
- [ ] Redefinir senha invalida sessões ativas conforme política.
**Relacionado:** [ADR-0004](../../adr/0004-identity.md)

## RF-AUTHENTICATION-004 — Verificação em duas etapas (MFA) — `MVP`
**Ator:** Gestor (política) / usuário
**Descrição:** por lidar com dados comerciais sensíveis, o sistema deve suportar verificação em duas etapas, configurável pelo escritório.
**Critérios de aceite:**
- [ ] MFA pode ser exigido para o escritório ou por usuário (recurso do Keycloak).
- [ ] Com MFA ativo, o login exige o segundo fator.
**Relacionado:** [ADR-0004](../../adr/0004-identity.md)
