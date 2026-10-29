# APTA VISÃO GERAL DO PRODUTO

> Documento de visão de produto. Descreve **o que** a Apta é, **para quem**, **por que existe** e **o que a torna diferente**. Não trata de implementação técnica, arquitetura ou cronograma de engenharia.

---

## O QUE É A APTA

A Apta é uma **plataforma B2B (SaaS)** para o mercado de consórcios. Ela não vende consórcio ao consumidor final: ela equipa as **empresas que vendem** escritórios de representação, correspondentes e consultores autônomos com a inteligência comercial para atender melhor o cliente final e fechar mais negócios.

Em uma frase: **a Apta transforma as tabelas de várias administradoras na melhor recomendação para o cliente e na melhor comissão para o escritório** e, acima de tudo, responde com dados a pergunta que todo cliente faz e que ninguém hoje responde bem: *"quando eu vou ser contemplado?"*

O nome carrega a tese do produto: tornar cada cliente **apto** mostrar a ele a cota certa, a estratégia de lance certa e o momento certo.

---

## O PROBLEMA QUE RESOLVEMOS

O consórcio é um dos maiores mercados de crédito do Brasil, mas é vendido com ferramentas que pararam no tempo:

- **Simulação manual e isolada.** O consultor abre o portal de cada administradora, uma por uma, e compara tabelas no olho ou numa planilha. Erra, demora e nunca garante que ofereceu a melhor opção.
- **A contemplação é uma caixa-preta.** A maior objeção de venda do consórcio *"demora pra ser contemplado"* é respondida com achismo. Ninguém mostra ao cliente, com dados, qual lance o coloca na frente e qual é a expectativa real de contemplação.
- **Lance decidido no escuro.** Lance livre, fixo, embutido a estratégia que define o sucesso da compra é escolhida sem apoio de dados.
- **Venda sem comparação honesta.** O cliente não sabe quando consórcio é melhor que financiamento. Sem essa clareza, a venda perde confiança e o pós-venda vira arrependimento e cancelamento.
- **Pós-venda à deriva.** Depois da assinatura, o acompanhamento da cota (parcelas, contemplação, uso da carta) acontece em grupos de WhatsApp e cadernos. Isso gera inadimplência e churn.

O mercado já tem **sistema de administradora** (que só vende os grupos daquela administradora), **CRM genérico** (que não entende consórcio) e até **simuladores multi-administradora** — mas todos param na parcela e no prazo. O que ninguém entrega é a **inteligência de contemplação**: responder, com dados, quando e como o cliente será contemplado. É essa lacuna, e não a simulação em si, que a Apta ocupa.

---

## PARA QUEM É (PÚBLICO-ALVO)

**Cliente que assina (ICP):** escritórios e correspondentes que representam **duas ou mais administradoras** e vivem de comissão sobre vendas. É para eles que a Apta gera retorno direto: mais conversão = mais comissão.

Personas dentro do cliente:

| Persona | Papel | O que a Apta entrega a ele |
| :--- | :--- | :--- |
| **Gestor do escritório** | Assina o contrato, define comissões e metas | Padronização da operação, visão da carteira, controle de produtividade |
| **Consultor / Vendedor** | Usuário do dia a dia, atende o cliente final | Simulação instantânea, recomendação de lance, argumento de venda com dados |
| **Cliente final** | Consumidor (uso **indireto**) | Recebe uma proposta clara, comparável e com expectativa realista de contemplação |

O cliente final **não é usuário** da plataforma ele é o beneficiário da qualidade da venda. Esse é o coração do modelo B2B: a Apta atende empresas para que elas atendam melhor seus clientes.

---

## PROPOSTA DE VALOR E POSICIONAMENTO

A Apta se posiciona como **o copiloto comercial do escritório de consórcio**. Não é um portal de administradora nem um CRM genérico: é a inteligência que falta entre as tabelas e a venda.

Três promessas sustentam o posicionamento:

1. **Para o cliente final a melhor decisão.** A cota certa, pela estratégia certa, com expectativa honesta de contemplação.
2. **Para o escritório a melhor venda.** Mais conversão, ticket maior e a comissão otimizada em cada simulação.
3. **Para o mercado transparência.** Tirar a contemplação da caixa-preta é o que nos diferencia de tudo que existe hoje.

**O diferencial central (o que não se vê no mercado): Inteligência de Contemplação.** Enquanto o resto do mercado simula apenas *parcela e prazo*, a Apta modela a **probabilidade e a estratégia de contemplação** transformando a maior fraqueza comercial do produto consórcio no principal motivo para fechar.

---

## PILARES DO PRODUTO

Os módulos abaixo são descritos como **capacidades de produto**, não como componentes de software.

### 1. Catálogo de Ofertas
O escritório sobe e mantém as **tabelas de venda** das administradoras que representa (via importação simples CSV). Taxa de administração, prazos, faixas de carta, regras de lance e comissões ficam centralizados e versionados. O dado comercial pertence ao cliente; a Apta apenas o operacionaliza. *Esta é a base que torna o produto lançável sem depender de integração com nenhuma administradora.*

### 2. Motor de Simulação Multi-administradora
Dado o objetivo do cliente (valor do bem, prazo, capacidade de parcela), a Apta **varre todas as ofertas do catálogo** e entrega um **ranking** por dois critérios simultâneos: melhor para o cliente (menor custo / menor parcela) e melhor para o escritório (melhor comissão). Acaba a comparação manual.

### 3. Inteligência de Contemplação *(diferencial)*
O módulo que responde *"quando eu vou ser contemplado?"* com dados, não com achismo. Modela expectativa de contemplação por sorteio e por lance, considerando o perfil do grupo e o histórico, e traduz isso em uma leitura clara para o cliente. É o pilar que redefine a conversa de venda.

### 4. Otimizador de Lance
A partir do objetivo do cliente *ser contemplado rápido* ou *pagar menos* a Apta recomenda **tipo e valor de lance** (livre, fixo, embutido) e mostra o trade-off. Conecta-se diretamente à Inteligência de Contemplação.

### 5. Comparador Consórcio × Financiamento
Uma ferramenta de **decisão honesta** que mostra, lado a lado, quando o consórcio vence um financiamento e quando não vence. Gera confiança, qualifica o cliente e protege o escritório contra venda mal-feita (e cancelamento futuro).

### 6. Gestão de Carteira e Pós-venda
Do fechamento em diante: formalização da proposta, acompanhamento de parcelas, status de contemplação e uso da carta de crédito. Reduz inadimplência e churn e transforma a Apta de "ferramenta de simulação" em **sistema operacional do escritório**.

---

## COMO A APTA GERA E CAPTURA VALOR

**Modelo:** assinatura SaaS B2B, cobrada por escritório (com faixas por número de consultores/assentos e por módulos). O valor para o cliente é direto e mensurável mais vendas fechadas e comissão otimizada, o que sustenta disposição a pagar recorrente.

**Lógica de expansão de receita:** começa pela simulação (dor imediata, barreira zero) e sobe para inteligência e pós-venda (onde o cliente já não quer mais sair). Quanto mais carteira o escritório gerencia na Apta, maior o custo de troca.

---

## PRINCÍPIOS DE PRODUTO

1. **O dado comercial é do cliente.** A Apta nunca expõe a tabela ou a comissão de um escritório a outro. Confidencialidade é pré-requisito de confiança no B2B.
2. **Transparência vende mais que promessa.** Preferimos mostrar a expectativa honesta de contemplação a inflar expectativa porque venda honesta não cancela.
3. **O dono da relação é o escritório.** A Apta potencializa o consultor; não desintermedia nem fala direto com o cliente final.
4. **Simulação exata, não "estimativa".** Como os termos do consórcio são fixos, a simulação da Apta é precisa e isso é um ativo de marca.
5. **Conformidade por padrão.** As regras regulatórias (BACEN / Lei 11.795) são parte do produto, não um adendo.

---

## COMO MEDIMOS SUCESSO

**Métrica-Norte (North Star):** **taxa de conversão de simulação em venda fechada** dentro da plataforma porque ela prova que a Apta não só informa, mas faz o escritório vender mais.

Métricas de apoio:

- Valor em cartas de crédito simuladas → efetivamente vendidas
- Cotas ativas sob gestão na plataforma (proxy de dependência/retenção)
- Adoção da Inteligência de Contemplação por consultor (uso do diferencial)
- Churn de escritórios e expansão de receita por cliente
