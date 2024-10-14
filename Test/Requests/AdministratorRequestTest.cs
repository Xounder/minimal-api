using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Domain.ModelViews;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class AdministradorRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }
    
    [TestMethod]
    public async Task TestGetSetProperties()
    {
        var loginDTO = new LoginDTO{
            Email = "adm@teste.com",
            Password = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8,  "Application/json");

        var response = await Setup.client.PostAsync("/administradores/login", content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var loggedAdm = JsonSerializer.Deserialize<LoggedAdministrator>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(loggedAdm?.Email ?? "");
        Assert.IsNotNull(loggedAdm?.Role ?? "");
        Assert.IsNotNull(loggedAdm?.Token ?? "");
    }
}