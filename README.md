# Minimal API

A minimalist API for vehicle and administrator management. This project is built with .NET and uses the Minimal API architecture to provide a simple and effective interface.

## Endpoints

### Home

| Method | Endpoint | Description | Response |
|--------|----------|-------------|----------|
| GET    | `/`      | Returns a welcome message | `Home` |

### Administrators

| Method | Endpoint                  | Description                          | Request Body        | Response                     |
|--------|---------------------------|------------------------------------|---------------------|------------------------------|
| POST   | `/administradores/login`   | Logs in an administrator            | `LoginDTO`          | `LoggedAdministrator`        |
| GET    | `/administradores`         | Returns a list of administrators    | Query Params: `page` | `List<AdministratorModelView>` |
| GET    | `/administradores/{id}`    | Returns a specific administrator     | N/A                 | `AdministratorModelView`     |
| POST   | `/administradores`         | Creates a new administrator         | `AdministratorDTO`   | `AdministratorModelView`     |

### Vehicles

| Method | Endpoint                  | Description                          | Request Body        | Response                     |
|--------|---------------------------|------------------------------------|---------------------|------------------------------|
| GET    | `/veiculos`               | Returns a list of vehicles          | Query Params: `page` | `List<VehicleDTO>`          |
| GET    | `/veiculos/{id}`          | Returns a specific vehicle          | N/A                 | `VehicleDTO`                |
| POST   | `/veiculos`               | Adds a new vehicle                  | `VehicleDTO`       | `VehicleDTO`                |
| PUT    | `/veiculos/{id}`          | Updates an existing vehicle         | `VehicleDTO`       | `VehicleDTO`                |
| DELETE | `/veiculos/{id}`          | Removes an existing vehicle         | N/A                 | `204 No Content`             |

