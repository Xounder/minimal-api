using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Db;

namespace Test.Domain.Entities;
[TestClass]
public class AdministratorServiceTest
{
    private DbContexto CreateTestContext()
    {
        // Usando banco de dados em memória para testes
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        return new DbContexto(options, "TestDatabase");
    }

    [TestMethod]
    public void TestSaveAdministrator()
    {
        var context = CreateTestContext();
        var adm = new Administrator
        {
            Email = "teste@teste.com",
            Password = "teste",
            Role = "Adm"
        };

        var administratorService = new AdministratorService(context);

        administratorService.Include(adm);

        Assert.AreEqual(1, administratorService.All(1).Count());
    }

    [TestMethod]
    public void TestFindById()
    {
        var context = CreateTestContext();
        var adm = new Administrator
        {
            Email = "teste@teste.com",
            Password = "teste",
            Role = "Adm"
        };

        var administratorService = new AdministratorService(context);

        administratorService.Include(adm);
        var databaseAdm = administratorService.FindById(adm.Id);

        Assert.AreEqual(adm.Id, databaseAdm?.Id);
    }
}
