# ADR-0008: Contrato e estilo da API REST

## Status
Aceito

## Contexto
A Apta expõe um backend para o frontend e, no futuro, possivelmente para integrações. Precisamos de um contrato de API previsível e padronizado: respostas consistentes, status HTTP semânticos e erros que o cliente consiga tratar e investigar sem adivinhação. Hoje isso é implícito, mas não está registrado como decisão.

## Decisão
API **REST orientada a recursos**, com as seguintes regras:

- **Respostas sempre explícitas e semânticas.** Todo endpoint devolve a representação esperada com o **status HTTP correto** (`200/201/204` no sucesso; `4xx` para erro do cliente; `5xx` para falha do servidor), seguindo a semântica de HTTP.
- **Contrato de erro único.** Em qualquer erro, a resposta é um objeto `{ code, description }` — nada de formato ad hoc por endpoint.
- **Códigos de erro catalogados.** `code` segue o padrão `#RESOURCE-ERROR-XXXXX`, onde:
  - `RESOURCE` é o recurso em maiúsculas (ex.: `PRODUCT`, `OFFER`);
  - `ERROR` é literal;
  - `XXXXX` são os **5 primeiros caracteres, em maiúsculas, de um hash MD5** de uma string — garantindo um código único e estável, nunca "inventado" na hora.

  Cada código é registrado em um **catálogo estático** no código e descrito na **documentação oficial de erros**, que explica o que o erro significa e o que fazer.
- **Versionamento explícito** da API (ex.: prefixo `/v1`), evoluindo de forma retrocompatível dentro da mesma versão.
- **Alinhado às convenções do time**: controllers não contêm lógica — apenas traduzem o `Result` dos handlers para o status HTTP e o corpo correspondentes.

## Consequências

### Positivas
- O cliente trata erros de forma uniforme e programática (switch no `code`), sem precisar parsear mensagens.
- Cada erro é rastreável: o `code` leva direto à documentação oficial, com causa e ação.
- Códigos derivados de hash evitam colisão e "números mágicos" escolhidos à mão.

### Negativas
- Exige disciplina para catalogar cada novo erro e manter a documentação oficial em dia.
- O `code` por hash é menos legível que um sequencial (mitigado pelo catálogo e pela documentação).

### Neutras
- O contrato `{ code, description }` é próprio; não adotamos `application/problem+json` (RFC 9457) por ora.

## Alternativas Consideradas
- **Problem Details (RFC 9457 / `problem+json`)**: não adotado — preferimos um contrato enxuto e um catálogo próprio de códigos, com documentação dedicada. Pode ser reavaliado se surgir necessidade de interoperabilidade externa.
- **Mensagens de erro livres por endpoint**: rejeitado — inviabiliza tratamento programático e documentação consistente.

## Referências
- OpenAPI Specification: https://spec.openapis.org/
- RFC 9110 (HTTP Semantics): https://www.rfc-editor.org/rfc/rfc9110
- ADR-0001
