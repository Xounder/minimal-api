using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.DTOs;

namespace Test.Mocks;

public class AdministratorServiceMock : IAdministratorService
{
    private static List<Administrator> Administrators = new List<Administrator>(){
        new Administrator{
            Id = 1,
            Email = "adm@teste.com",
            Password = "123456",
            Role = "Adm"
        },
        new Administrator{
            Id = 2,
            Email = "editor@teste.com",
            Password = "123456",
            Role = "Editor"
        }
    };

    public Administrator? FindById(int id)
    {
        return Administrators.Find(a => a.Id == id);
    }

    public Administrator Include(Administrator administrator)
    {
        administrator.Id = Administrators.Count() + 1;
        Administrators.Add(administrator);

        return administrator;
    }

    public Administrator? Login(LoginDTO loginDTO)
    {
        return Administrators.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
    }

    public List<Administrator> All(int? pagina)
    {
        return Administrators;
    }
}
