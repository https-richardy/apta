# RF — SIMULAÇÃO

> Parte dos [requisitos funcionais](README.md) da Apta. Convenções e status no [índice](README.md). Isolamento por tenant ([ADR-0003](../../adr/0003-multi-tenancy.md)) e auditoria ([ADR-0010](../../adr/0010-audit.md)) valem para todos os RFs.
>
> *Pilar 2. Acaba com a comparação manual, uma administradora por vez.*

---

## RF-SIMULATIONS-001 — Simular a partir do objetivo do cliente — `MVP`
**Ator:** Consultor
**Descrição:** o sistema deve gerar uma simulação a partir dos dados do objetivo do cliente (valor do bem/carta desejada, prazo pretendido, capacidade de parcela).
**Critérios de aceite:**
- [ ] A simulação considera apenas ofertas ativas do catálogo do escritório que atendem aos critérios.
- [ ] Objetivo informado de forma incompleta recebe erro claro, não resultado silenciosamente errado.
**Relacionado:** overview pilar 2; persona Consultora (Juliana)

## RF-SIMULATIONS-002 — Calcular a composição da parcela e da carta — `MVP`
**Ator:** sistema
**Descrição:** para cada oferta elegível, o sistema deve calcular valor da carta, composição da parcela (fundo comum + taxa de administração + fundo de reserva + seguro) e custo total do plano.
**Critérios de aceite:**
- [ ] A composição da parcela segue os termos da oferta vigente.
- [ ] Os valores são apresentados de forma detalhada (não só o total).
**Relacionado:** glossário (Parcela, Taxa de administração, Fundo comum/reserva)

## RF-SIMULATIONS-003 — Ranquear ofertas por dois critérios — `MVP`
**Ator:** sistema
**Descrição:** o resultado deve ser um ranking simultâneo por **melhor para o cliente** (menor custo/parcela) e **melhor comissão para o escritório**.
**Critérios de aceite:**
- [ ] É possível ordenar/ver o resultado pelos dois critérios.
- [ ] Cada item do ranking identifica a administradora e a oferta de origem.
**Relacionado:** overview pilar 2; princípio "o dado comercial é do cliente"

## RF-SIMULATIONS-004 — Simulação precisa e determinística — `MVP`
**Ator:** sistema
**Descrição:** como os termos do consórcio são fixos, a simulação deve ser exata e reprodutível: mesmos insumos e mesma versão de tabela produzem sempre o mesmo resultado.
**Critérios de aceite:**
- [ ] Reexecutar a simulação com os mesmos insumos dá resultado idêntico.
- [ ] O resultado registra qual versão da tabela foi usada (RF-OFFER-CATALOG-003).
**Relacionado:** princípio 4 ("simulação exata, não estimativa")

## RF-SIMULATIONS-005 — Não expor comissão ao cliente final — `MVP`
**Ator:** sistema
**Descrição:** a saída da simulação destinada ao cliente final (proposta/apresentação) nunca deve conter a comissão do escritório.
**Critérios de aceite:**
- [ ] A visão interna (consultor) mostra a comissão; a visão/compartilhamento para o cliente, não.
**Relacionado:** princípio "o dado comercial é do cliente"; objeção da persona Consultora

## RF-SIMULATIONS-006 — Gerar proposta a partir de uma simulação — `MVP`
**Ator:** Consultor
**Descrição:** o consultor deve poder transformar uma opção escolhida do ranking em uma proposta clara para enviar ao cliente.
**Critérios de aceite:**
- [ ] A proposta reúne oferta escolhida, carta, parcela e prazo de forma apresentável.
- [ ] A simulação de origem fica registrada (rastreável à proposta).
**Relacionado:** [rf-portfolio.md](rf-portfolio.md) (formalização)
