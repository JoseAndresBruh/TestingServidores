using System.Net;
using System.Net.Http.Json;
using Api.Features.Usuarios;
using FluentAssertions;

namespace Api.Tests;

public class UpdateUserTests : IntegrationTestBase
{
    [Fact]
    public async Task UpdateUser_DeberiaActualizarYRetornarNoContent_CuandoUsuarioExiste()
    {
        // Arrange
        var crearComando = new CrearUsuarioComando("Carlos Viejo", "viejo@test.com");
        var crearResponse = await Client.PostAsJsonAsync("/usuarios", crearComando);
        
        // CORRECCIÓN: Leer la respuesta directamente como string y quitarle las comillas
        var userIdString = await crearResponse.Content.ReadAsStringAsync();
        Guid userId = Guid.Parse(userIdString.Trim('"'));

        // Act
        var updateComando = new UpdateUserCommand(userId, "Carlos Nuevo", "nuevo@test.com");
        var updateResponse = await Client.PutAsJsonAsync($"/usuarios/{userId}", updateComando);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar en la BD
        var verifyResponse = await Client.GetAsync($"/usuarios/{userId}");
        var usuarioActualizado = await verifyResponse.Content.ReadFromJsonAsync<UserDto>();
        
        usuarioActualizado.Should().NotBeNull();
        usuarioActualizado!.Nombre.Should().Be("Carlos Nuevo");
        usuarioActualizado.Email.Should().Be("nuevo@test.com");
    }

    [Fact]
    public async Task UpdateUser_DeberiaRetornarNotFound_CuandoUsuarioNoExiste()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();
        var updateComando = new UpdateUserCommand(idInexistente, "Fantasma", "ghost@test.com");

        // Act
        var response = await Client.PutAsJsonAsync($"/usuarios/{idInexistente}", updateComando);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}