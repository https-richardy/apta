# OVERVIEW

A Apta é uma plataforma B2B (SaaS) para o mercado de consórcios: ela não vende consórcio ao consumidor final, mas equipa os escritórios representantes que vendem cotas de duas ou mais administradoras com a inteligência comercial para atender melhor o cliente final e fechar mais negócios, transformando as tabelas de várias administradoras na melhor recomendação para o cliente e na melhor comissão para o escritório. Essa API dá forma a essa engrenagem através de alguns conceitos centrais: o **Escritório**, o representante que assina a plataforma e dono exclusivo dos seus dados comerciais, nunca expostos a outro escritório; a **Administradora**, a empresa autorizada pelo BACEN a organizar e gerir os grupos de consórcio, cujas ofertas o escritório representa; a **Oferta**, a tabela de venda de uma administradora (taxa de administração, prazos, faixas de carta, regras de lance e comissões), mantida e versionada pelo próprio escritório; a **Simulação**, que, dado o objetivo do cliente, varre todas as ofertas do catálogo e as ranqueia pelo que é melhor para o cliente e pelo que é melhor para o escritório; e a **Cota**, a participação vendida e acompanhada na carteira do escritório até a contemplação e o uso da carta de crédito.

## AUTENTICAÇÃO

Toda requisição a esta API deve carregar um token de acesso válido, representando o usuário autenticado, no cabeçalho `Authorization`. É esse token obtido através do fluxo OAuth2 de Authorization Code que a plataforma usa para saber quem está fazendo a chamada e quais permissões essa pessoa carrega consigo; requisições sem um token válido, ou com um token que não contempla a permissão exigida pelo recurso, são recusadas antes de qualquer regra de negócio ser executada.

## PADRONIZAÇÃO

Toda chamada a esta API resulta em um de dois formatos possíveis: a resposta esperada para aquele recurso, ou um erro padronizado no formato `{ code, description }`, onde `code` identifica de forma unívoca o tipo de erro ocorrido e `description` traz uma descrição legível do que aconteceu. Essa padronização vale para qualquer endpoint da API, independentemente do domínio de negócio ou do motivo da falha.
