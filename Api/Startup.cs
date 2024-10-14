using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.Services;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.ModelViews;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration.GetSection("Jwt").ToString() ?? "";
    }

    private string key;
    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        
        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters{
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement 
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference 
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        services.AddDbContext<DbContexto>(options => {
            options.UseSqlServer(Configuration?.GetConnectionString("SqlServer"));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Administradores
            string GenerateTokenJwt(Administrator administrator)
            {
                if(string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrator.Email),
                    new Claim("Role", administrator.Role),
                    new Claim(ClaimTypes.Role, administrator.Role)
                };

                var token = new JwtSecurityToken(
                    claims: claims, 
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            AdministratorModelView createAdministratorModelView(Administrator administrator)
            {
                return new AdministratorModelView{
                        Id = administrator.Id,
                        Email = administrator.Email,
                        Role = administrator.Role
                    };
            }

            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, 
                                IAdministratorService administratorService) => {
                var adm = administratorService.Login(loginDTO);
                if (adm!= null)
                {
                    string token = GenerateTokenJwt(adm);
                    return Results.Ok(new LoggedAdministrator
                    {
                        Email = adm.Email,
                        Role = adm.Role,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Administradores");

            endpoints.MapGet("/administradores", ([FromQuery] int? page, 
                                IAdministratorService administratorService) => {
                var adms = new List<AdministratorModelView>();
                var administrators = administratorService.All(page);
                foreach(var adm in administrators)
                {
                    adms.Add(createAdministratorModelView(adm));
                }
                return Results.Ok(adms);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");

            endpoints.MapGet("/administradores/{id}", ([FromRoute] int id,
                                IAdministratorService administratorService) => {
                var administrator = administratorService.FindById(id);
                if (administrator == null) return Results.NotFound();
                return Results.Ok(createAdministratorModelView(administrator));
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");

            endpoints.MapPost("/administradores", ([FromBody] AdministratorDTO administratorDTO, 
                                IAdministratorService administratorService) => {
                var validation = new ValidationErrors{
                    Messages = new List<string>()
                };

                if (string.IsNullOrEmpty(administratorDTO.Email))
                    validation.Messages.Add("Email não pode ser vazio");
                if (string.IsNullOrEmpty(administratorDTO.Password))
                    validation.Messages.Add("Senha não pode ser vazia");
                if (administratorDTO.Role == null)
                    validation.Messages.Add("Perfil não pode ser vazio");

                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                var administrator = new Administrator {
                    Email = administratorDTO.Email,
                    Password = administratorDTO.Password,
                    Role = administratorDTO.Role.ToString() ?? Role.Editor.ToString()
                };
                administratorService.Include(administrator);

                return Results.Created($"/administrador/{administrator.Id}", 
                                createAdministratorModelView(administrator));
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");
            #endregion

            #region Veiculos
            ValidationErrors validationDTO(VehicleDTO vehicleDTO)
            {
                var validation = new ValidationErrors{
                    Messages = new List<string>()
                };

                if (string.IsNullOrEmpty(vehicleDTO.Name))
                    validation.Messages.Add("O nome não pode ser vazio.");
                    
                if (string.IsNullOrEmpty(vehicleDTO.Make))
                    validation.Messages.Add("A marca não pode ficar em branco.");

                if (vehicleDTO.Year < 1950)
                    validation.Messages.Add("Veículo muito antigo, aceito somente anos superiores a 1950.");

                return validation;
            }

            endpoints.MapPost("/veiculos", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
                var validation = validationDTO(vehicleDTO);
                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                var vehicle = new Vehicle {
                    Name = vehicleDTO.Name,
                    Make = vehicleDTO.Make,
                    Year = vehicleDTO.Year
                };
                vehicleService.Include(vehicle);

                return Results.Created($"/veiculos/{vehicle.Id}", vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags("Veiculos");

            endpoints.MapGet("/veiculos", ([FromQuery] int? page, IVehicleService vehicleService) => {
                var vehicle = vehicleService.All(page);

                return Results.Ok(vehicle);
            }).RequireAuthorization().WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                var vehicle = vehicleService.FindById(id);
                if (vehicle == null) return Results.NotFound();
                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
                var vehicle = vehicleService.FindById(id);
                if (vehicle == null) return Results.NotFound();

                var validation = validationDTO(vehicleDTO);
                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                vehicle.Name = vehicleDTO.Name;
                vehicle.Make = vehicleDTO.Make;
                vehicle.Year = vehicleDTO.Year;

                vehicleService.Update(vehicle);

                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                var vehicle = vehicleService.FindById(id);
                if (vehicle == null) return Results.NotFound();

                vehicleService.Delete(vehicle);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");
            #endregion
        });
    }
}