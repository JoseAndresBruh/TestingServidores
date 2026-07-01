using System.Net.Http.Json;
using Api.Features.Usuarios;
using FluentAssertions;

namespace Api.Tests;

public class ListarUsuariosTests : IntegrationTestBase
{
    [Fact]
    public async Task ListarUsuarios_DeberiaRetornar200OK()
    {
        // Act
        var response = await Client.GetAsync("/usuarios?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}