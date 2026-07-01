namespace Api.Features.Usuarios;

public record PagedList<T>(List<T> Items, int Page, int PageSize, int TotalCount);
public record UserDto(Guid Id, string Nombre, string Email);