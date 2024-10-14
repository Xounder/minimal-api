using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class VehicleTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        var vehicle = new Vehicle();
        vehicle.Id = 1;
        vehicle.Name = "Uno";
        vehicle.Make = "Fiat";
        vehicle.Year = 1999;

        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("Uno", vehicle.Name);
        Assert.AreEqual("Fiat", vehicle.Make);
        Assert.AreEqual(1999, vehicle.Year);
    }
}