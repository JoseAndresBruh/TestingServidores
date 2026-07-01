using System.Diagnostics;
using MediatR;

namespace Api.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();
        var response = await next();
        timer.Stop();

        var elapsed = timer.ElapsedMilliseconds;
        
        if (elapsed > 500)
        {
            logger.LogWarning("Advertencia de Rendimiento: Solicitud lenta detectada. {RequestName} tomó {Elapsed} ms.", typeof(TRequest).Name, elapsed);
        }
        
        return response;
    }
}