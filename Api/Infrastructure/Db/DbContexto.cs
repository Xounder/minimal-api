using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastructure.Db;

public class DbContexto : DbContext
{

    private readonly IConfiguration _appSettingsConfiguration;

    // Test Database InMemory
    public DbContexto(DbContextOptions<DbContexto> options, string databaseTest) : base(options) { }
    
    public DbContexto(IConfiguration appSettingsConfiguration)
    {
        _appSettingsConfiguration = appSettingsConfiguration;
    }

    public DbSet<Administrator> Administrators { get; set; } = default!;
    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator{
                Id = 1,
                Email = "administrador@teste.com",
                Password = "123456",
                Role = "Adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _appSettingsConfiguration.GetConnectionString("SqlServer")?.ToString();
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}