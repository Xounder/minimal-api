using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Interfaces;
using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

namespace minimal_api.Domain.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly DbContexto _context;

        public AdministratorService(DbContexto context)
        {
            _context = context;
        }

        public List<Administrator> All(int? page)
        {
            var query = _context.Administrators.AsQueryable();
            
            int itensPerPage = 10;

            if (page != null)
            {
                query = query.Skip(((int)page - 1) * itensPerPage).Take(itensPerPage);
            }

            return query.ToList();
        }

        public Administrator? FindById(int id)
        {
            return _context.Administrators.Where(v => v.Id == id).FirstOrDefault();
        } 

        public Administrator Include(Administrator administrator)
        {
            _context.Administrators.Add(administrator);
            _context.SaveChanges();

            return administrator;
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && 
                                                         a.Password == loginDTO.Password)
                                                         .FirstOrDefault();
            return adm; 
        }
    }
}