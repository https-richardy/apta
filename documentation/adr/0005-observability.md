# ADR-0005: Observabilidade com Seq e Sentry

## Status
Aceito

## Contexto
Precisamos de visibilidade operacional desde o início. Além disso, a tendência é o software evoluir para uma topologia mais distribuída no futuro, cenário em que a correlação de logs se torna essencial.

## Decisão
Usar **Seq** como destino de **logs estruturados** do backend .NET e **Sentry** para **monitoramento de erros** (captura e agrupamento de exceções, alertas e rastreio por release). Seq cobre o log estruturado e a correlação; o Sentry cobre a detecção e o diagnóstico de erros em produção.

## Consequências

### Positivas
- Log estruturado com busca e consulta poderosas; excelente integração no ecossistema .NET.
- Preparado para a evolução: centraliza e correlaciona logs quando o sistema se distribuir.
- Sentry captura e agrupa exceções em produção, com alertas e associação a releases — acelera a detecção e o diagnóstico de erros, complementando os logs do Seq.

### Negativas
- Adiciona um componente de infraestrutura a operar (já previsto).
- Sentry envia dados de erro a um serviço externo; stack traces/payloads podem conter PII, exigindo scrubbing antes do envio (ver ADR-0006).

### Neutras
- Métricas e tracing (ex.: OpenTelemetry) podem complementar no futuro, sem conflito com esta decisão.

## Implementação
O backend já integra as duas frentes definidas nesta decisão:
- **Logs estruturados** via Serilog, com saída para console e para o **Seq**.
- **Monitoramento de erros e performance** via **Sentry** (integração ASP.NET Core), com captura automática de exceções e identificação do ambiente de origem; os eventos de log do Serilog também são encaminhados ao Sentry.

As integrações são configuradas por ambiente (DSN do Sentry e URL do coletor do Seq, mantidos fora do versionamento) e ficam ativas apenas em desenvolvimento e produção, desligadas durante os testes para não impactar a suíte.

## Alternativas Consideradas
- **Stack ELK**: mais pesada do que o necessário para a escala atual.
- **Apenas logs em arquivo**: insuficiente para a evolução distribuída.

## Referências
- Seq: https://docs.datalust.co/docs
- Sentry para .NET: https://docs.sentry.io/platforms/dotnet/
- The Twelve-Factor App — Logs: https://12factor.net/logs
- OpenTelemetry: https://opentelemetry.io/docs/
