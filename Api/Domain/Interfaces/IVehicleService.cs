using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> All(int? page = 1, string? name = null, string? make = null);
        Vehicle? FindById(int id);
        void Include(Vehicle vehicle);
        void Update(Vehicle vehicle);
        void Delete(Vehicle vehicle);
    }
}