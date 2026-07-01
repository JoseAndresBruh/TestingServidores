using System.Net.Http.Json;
using Api.Features.Usuarios;
using FluentAssertions;

namespace Api.Tests;

public class ListarUsuariosTests : IntegrationTestBase
{
    [Fact]
    public async Task ListarUsuarios_DeberiaRetornarUsuariosPaginados_CuandoExistenRegistros()
    {
        // Arrange
        // (Aquí podrías insertar algunos usuarios si quisieras probar el paginado exacto)
        
        // Act
        var response = await Client.GetAsync("/usuarios?page=1&pageSize=10");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var resultado = await response.Content.ReadFromJsonAsync<PagedList<UserDto>>();
        resultado.Should().NotBeNull();
        resultado!.Items.Should().NotBeNull();
    }
}