using Api.Domain.Entities;
using Api.Infrastructure;
using FluentValidation;
using MediatR;

namespace Api.Features.Usuarios;

public record CrearUsuarioComando(string Nombre, string Email) : IRequest<Guid>;

public class CrearUsuarioValidador : AbstractValidator<CrearUsuarioComando>
{
    public CrearUsuarioValidador()
    {
        RuleFor(x => x.Nombre).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).EmailAddress();
    }
}

public class ManejadorCrearUsuario(AppDbContext dbContext) : IRequestHandler<CrearUsuarioComando, Guid>
{
    public async Task<Guid> Handle(CrearUsuarioComando request, CancellationToken cancellationToken)
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Nombre = request.Nombre, Email = request.Email };
        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);
        return usuario.Id;
    }
}