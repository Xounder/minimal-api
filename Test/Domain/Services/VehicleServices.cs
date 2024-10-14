using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Db;

namespace Test.Domain.Entities;
[TestClass]
public class VehicleServiceTest
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
    public void TestSaveVehicle()
    {
        var context = CreateTestContext();
        var vehicle = new Vehicle
        {
            Name = "Uno",
            Make = "Fiat",
            Year = 1999
        };

        var vehicleService = new VehicleService(context);

        vehicleService.Include(vehicle);

        Assert.AreEqual(1, vehicleService.All(1).Count());
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        var context = CreateTestContext();
        var vehicle = new Vehicle
        {
            Name = "Uno",
            Make = "Fiat",
            Year = 1999
        };

        var vehicleService = new VehicleService(context);

        vehicleService.Include(vehicle);
        var databaseAdm = vehicleService.FindById(vehicle.Id);

        Assert.AreEqual(vehicle.Id, databaseAdm?.Id);
    }
}
