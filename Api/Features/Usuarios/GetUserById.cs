using Api.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Usuarios;

// DTO
public record UserResponse(Guid Id, string Nombre, string Email);

// Query
public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;

// Handler
public class GetUserByIdHandler(AppDbContext dbContext) : IRequestHandler<GetUserByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.Usuarios
            .Where(u => u.Id == query.Id)
            .Select(u => new UserResponse(u.Id, u.Nombre, u.Email))
            .FirstOrDefaultAsync(cancellationToken);
    }
}

// Extensión para el endpoint
public static class GetUserByIdEndpoint
{
    public static void MapGetUserById(this IEndpointRouteBuilder app) =>
        app.MapGet("/usuarios/{id:guid}", async (Guid id, ISender sender) =>
        {
            var user = await sender.Send(new GetUserByIdQuery(id));
            return user is not null ? Results.Ok(user) : Results.NotFound();
        });
}