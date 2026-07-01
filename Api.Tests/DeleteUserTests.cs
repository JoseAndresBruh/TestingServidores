using System.Net;
using System.Net.Http.Json;
using Api.Features.Usuarios;
using FluentAssertions;

namespace Api.Tests;

public class DeleteUserTests : IntegrationTestBase
{
    [Fact]
    public async Task DeleteUser_DeberiaOcultarUsuario_CuandoSeEjecutaSoftDelete()
    {
        // 1. Arrange: Crear usuario
        var crearComando = new CrearUsuarioComando("Borrame", "borrar@test.com");
        var crearResponse = await Client.PostAsJsonAsync("/usuarios", crearComando);
        var resultadoCreacion = await crearResponse.Content.ReadFromJsonAsync<dynamic>();
        Guid userId = Guid.Parse(resultadoCreacion!.GetProperty("id").GetString()!);

        // 2. Act: Borrar usuario
        var deleteResponse = await Client.DeleteAsync($"/usuarios/{userId}");

        // 3. Assert: Verificar que responde 204 NoContent
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 4. Assert: Verificar que ya no se puede obtener (El Query Filter lo oculta)
        var getResponse = await Client.GetAsync($"/usuarios/{userId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}