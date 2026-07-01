using System.Net.Http.Json;
using Api.Features.Usuarios;
using FluentAssertions;

namespace Api.Tests;

public class CrearUsuarioTests : IntegrationTestBase
{
    [Fact]
    public async Task CrearUsuario_DeberiaGuardarUsuario_CuandoDatosSonValidos()
    {
        // Arrange
        var comando = new CrearUsuarioComando("Test User", "test@uleam.edu.ec");

        // Act
        var response = await Client.PostAsJsonAsync("/usuarios", comando);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}