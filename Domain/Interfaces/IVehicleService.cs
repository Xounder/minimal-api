using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace minimal_api.Domain.Interfaces
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