using Api.Infrastructure;
using Api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Api.Features.Usuarios;

public record DeleteUserCommand(Guid Id) : IRequest<bool>;

public class DeleteUserHandler(AppDbContext dbContext) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var usuario = await dbContext.Usuarios.FindAsync([request.Id], cancellationToken);
        
        if (usuario is null || usuario.IsDeleted)
        {
            return false;
        }

        usuario.IsDeleted = true;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public static class DeleteUserEndpoint
{
    public static void MapDeleteUser(this IEndpointRouteBuilder app) =>
        app.MapDelete("/usuarios/{id:guid}", async (Guid id, ISender sender) =>
        {
            var eliminado = await sender.Send(new DeleteUserCommand(id));
            return eliminado ? Results.NoContent() : Results.NotFound();
        });
}