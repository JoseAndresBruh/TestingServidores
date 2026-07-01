using System.Text.Json;
using MediatR;

namespace Api.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Manejando solicitud {RequestName}. Datos: {RequestData}", typeof(TRequest).Name, JsonSerializer.Serialize(request));
        
        var response = await next();
        
        logger.LogInformation("Solicitud {RequestName} completada con éxito.", typeof(TRequest).Name);
        return response;
    }
}