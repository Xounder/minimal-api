using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace minimal_api.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);
        Administrator Include(Administrator administrator);
        Administrator? FindById(int id);
        List<Administrator> All(int? page);
    }
}