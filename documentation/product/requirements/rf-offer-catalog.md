# RF — CATÁLOGO DE OFERTAS

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> *Pilar 1. Base lançável: não depende de integração com administradora.*

---

## RF-OFFER-CATALOG-001 — Importar tabela de ofertas via CSV — `MVP`
**Ator:** Gestor (ou back-office)
**Descrição:** o sistema deve permitir subir as tabelas de venda de uma administradora por importação de CSV.
**Critérios de aceite:**
- [ ] Mapear colunas do CSV para os campos de oferta (administradora, tipo de bem, prazo, faixa de carta, taxa de administração, regras de lance, comissão).
- [ ] Ao final, informar quantas ofertas foram criadas/atualizadas e quantas linhas foram rejeitadas.
**Relacionado:** overview pilar 1; persona Gestor

## RF-OFFER-CATALOG-002 — Validar o CSV importado — `MVP`
**Ator:** sistema
**Descrição:** o sistema deve validar cada linha do CSV e rejeitar as inválidas apontando a causa, sem abortar as válidas.
**Critérios de aceite:**
- [ ] Linha com campo obrigatório ausente, tipo inválido ou valor fora de faixa é rejeitada com mensagem clara (linha + causa).
- [ ] Linhas válidas são importadas mesmo havendo linhas inválidas no mesmo arquivo.
**Relacionado:** RF-OFFER-CATALOG-001

## RF-OFFER-CATALOG-003 — Versionar a tabela de ofertas — `MVP`
**Ator:** sistema
**Descrição:** cada importação/alteração relevante gera uma nova **versão** da tabela, preservando o histórico.
**Critérios de aceite:**
- [ ] É possível identificar qual versão da tabela estava vigente em uma data.
- [ ] Versões anteriores não são sobrescritas nem apagadas.
**Relacionado:** [ADR-0010](../../adr/0010-audit.md); overview ("tabelas versionadas")

## RF-OFFER-CATALOG-004 — Gerenciar oferta manualmente — `MVP`
**Ator:** Gestor (ou back-office)
**Descrição:** o sistema deve permitir cadastrar e editar uma oferta sem depender de CSV.
**Critérios de aceite:**
- [ ] Criar/editar oferta com todos os campos comerciais (taxa de administração, prazo, faixa de carta, regras de lance, comissão).
- [ ] Ativar/inativar oferta sem apagá-la (inativa não aparece em simulações novas).
**Relacionado:** RF-OFFER-CATALOG-001

## RF-OFFER-CATALOG-005 — Consultar o catálogo — `MVP`
**Ator:** Consultor, Gestor, back-office
**Descrição:** o sistema deve permitir listar, buscar e filtrar as ofertas do escritório.
**Critérios de aceite:**
- [ ] Filtrar por administradora, tipo de bem, prazo e faixa de carta.
- [ ] Só retorna ofertas do próprio escritório.
**Relacionado:** [ADR-0003](../../adr/0003-multi-tenancy.md)

## RF-OFFER-CATALOG-006 — Reimportação idempotente — `MVP`
**Ator:** sistema
**Descrição:** reimportar a mesma tabela não deve duplicar ofertas; deve atualizar as existentes e gerar nova versão.
**Critérios de aceite:**
- [ ] Reimportar o mesmo arquivo resulta no mesmo conjunto de ofertas (sem duplicatas).
**Relacionado:** RF-OFFER-CATALOG-001, RF-OFFER-CATALOG-003
