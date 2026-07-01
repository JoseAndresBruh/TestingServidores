using Api.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Usuarios;

// DTO de respuesta paginada
public record PagedList<T>(List<T> Items, int Page, int PageSize, int TotalCount);
public record UserDto(Guid Id, string Nombre, string Email);

// Consulta
public record ListarUsuariosQuery(int Page = 1, int PageSize = 10) : IRequest<PagedList<UserDto>>;

// Handler
public class ListarUsuariosHandler(AppDbContext dbContext) : IRequestHandler<ListarUsuariosQuery, PagedList<UserDto>>
{
    public async Task<PagedList<UserDto>> Handle(ListarUsuariosQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await dbContext.Usuarios.CountAsync(cancellationToken);
        
        var items = await dbContext.Usuarios
            .OrderBy(u => u.Nombre)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new UserDto(u.Id, u.Nombre, u.Email))
            .ToListAsync(cancellationToken);

        return new PagedList<UserDto>(items, query.Page, query.PageSize, totalCount);
    }
}

// Endpoint
public static class ListarUsuariosEndpoint
{
    public static void MapListarUsuarios(this IEndpointRouteBuilder app) =>
        app.MapGet("/usuarios", async (int? page, int? pageSize, ISender sender) =>
        {
            var query = new ListarUsuariosQuery(page ?? 1, pageSize ?? 10);
            return Results.Ok(await sender.Send(query));
        });
}