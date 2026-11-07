namespace Apta.WebUI.Interceptors;

public sealed class HttpErrorInterceptor(INotificationService notificationService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        HttpStatusCode statusCode;

        try
        {
            response = await base.SendAsync(request, cancellationToken);
            statusCode = response.StatusCode;
        }
        catch (HttpRequestException)
        {
            await notificationService.ShowErrorToastAsync("Sem conexão com o servidor", "Não foi possível comunicar com o servidor. Verifique a sua conexão e tente novamente.");
            throw;
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            await notificationService.ShowErrorToastAsync("Tempo de resposta esgotado", "O servidor demorou para responder. Tente novamente em instantes.");
            throw;
        }

        string notification = statusCode switch
        {
            HttpStatusCode.InternalServerError =>
                "Ocorreu um erro interno no servidor. Entre em contato com o suporte.",

            HttpStatusCode.ServiceUnavailable =>
                "O serviço está temporariamente indisponível. Entre em contato com o suporte.",

            HttpStatusCode.GatewayTimeout =>
                "O servidor demorou muito para responder. Entre em contato com o suporte.",

            _ => string.Empty
        };

        if (string.IsNullOrEmpty(notification) is false)
        {
            await notificationService.CloseAllToastsAsync();
            await notificationService.ShowErrorToastAsync("Ocorreu um erro inesperado", notification);
        }

        return response;
    }
}
