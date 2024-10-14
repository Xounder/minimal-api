using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);
        Administrator Include(Administrator administrator);
        Administrator? FindById(int id);
        List<Administrator> All(int? page);
    }
}