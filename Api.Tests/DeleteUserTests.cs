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
        
        // CORRECCIÓN: Leer la respuesta directamente como string y quitarle las comillas
        var userIdString = await crearResponse.Content.ReadAsStringAsync();
        Guid userId = Guid.Parse(userIdString.Trim('"'));

        // 2. Act: Borrar usuario
        var deleteResponse = await Client.DeleteAsync($"/usuarios/{userId}");

        // 3. Assert: Verificar que responde 204 NoContent
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 4. Assert: Verificar que ya no se puede obtener (El Query Filter lo oculta)
        var getResponse = await Client.GetAsync($"/usuarios/{userId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}