using Api.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Api.Features.Usuarios;

public record ListarUsuariosQuery(int Page, int PageSize) : IRequest<PagedList<UserDto>>;

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

public static class ListarUsuariosEndpoint
{
    public static void MapListarUsuarios(this IEndpointRouteBuilder app) =>
        app.MapGet("/usuarios", async (int? page, int? pageSize, ISender sender) =>
        {
            var query = new ListarUsuariosQuery(page ?? 1, pageSize ?? 10);
            return Results.Ok(await sender.Send(query));
        });
}