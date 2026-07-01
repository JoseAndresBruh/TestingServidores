using MediatR;
using Microsoft.EntityFrameworkCore;
using Api.Infrastructure;

namespace Api.Features.Usuarios;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;

public class GetUserByIdHandler(AppDbContext dbContext) : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.Usuarios
            .Where(u => u.Id == query.Id)
            .Select(u => new UserDto(u.Id, u.Nombre, u.Email))
            .FirstOrDefaultAsync(cancellationToken);
    }
}

public static class GetUserByIdEndpoint
{
    public static void MapGetUserById(this IEndpointRouteBuilder app) =>
        app.MapGet("/usuarios/{id:guid}", async (Guid id, ISender sender) =>
        {
            var user = await sender.Send(new GetUserByIdQuery(id));
            return user is not null ? Results.Ok(user) : Results.NotFound();
        });
}