# ADR-0001: Monólito em camadas (DDD/Clean)

## Status
Aceito

## Contexto
A Apta é um SaaS B2B de escala modesta no horizonte visível (no máximo cerca de uma centena de escritórios no primeiro ano), tocado por um time pequeno, sobre um domínio com fronteiras conhecidas (os pilares de produto). Precisamos de uma arquitetura simples de operar e evoluir, sem o custo de um sistema distribuído. As convenções de código do time já prescrevem camadas DDD/Clean (`Domain → Application → Infrastructure (+ IoC) → WebApi`) em uma solução única, e o backend já está estruturado assim.

## Decisão
Adotar um **monólito organizado em camadas** (DDD/Clean), mantendo a estrutura de pastas atual do backend (`Source/` com `Domain`, `Application`, `Infrastructure`, `Infrastructure.IoC`, `WebApi`). **Não** vamos, por ora, dividir o código em módulos por feature/bounded context: a modularização fica explicitamente adiada para quando a complexidade justificar.

## Consequências

### Positivas
- Deploy e depuração simples; sem latência de rede interna; iteração rápida.
- Aderente às convenções do time; curva de entrada baixa.

### Negativas
- Escalabilidade independente por parte é limitada (mitigado: a escala projetada não exige isso).
- Sem fronteiras de módulo explícitas, há risco de acoplamento entre áreas com o tempo (mitigação: disciplina de camadas e revisão de código).

### Neutras
- A modularização por feature permanece possível no futuro, como evolução natural, sem reescrever.

## Alternativas Consideradas
- **Monólito modular** (módulos por bounded context, como no corban): adiado — não traz retorno na escala atual e adiciona cerimônia.
- **Microserviços**: rejeitado — custo operacional de sistema distribuído sem necessidade; seria over-engineering para uma escala hipotética.

## Referências
- `documentation/product/overview.md`
