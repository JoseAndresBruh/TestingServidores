using Api.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Api.Features.Usuarios;

// 1. Comando
public record UpdateUserCommand(Guid Id, string Nombre, string Email) : IRequest<bool>;

// 2. Manejador
public class UpdateUserHandler(AppDbContext dbContext) : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var usuario = await dbContext.Usuarios.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (usuario is null)
        {
            return false;
        }

        usuario.Nombre = request.Nombre;
        usuario.Email = request.Email;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// 3. Endpoint
public static class UpdateUserEndpoint
{
    public static void MapUpdateUser(this IEndpointRouteBuilder app) =>
        app.MapPut("/usuarios/{id:guid}", async (Guid id, UpdateUserCommand comando, ISender sender) =>
        {
            // Asegurar que el ID de la URL coincida con el comando, o forzarlo
            var comandoConId = comando with { Id = id };
            var actualizado = await sender.Send(comandoConId);
            
            return actualizado ? Results.NoContent() : Results.NotFound();
        });
}