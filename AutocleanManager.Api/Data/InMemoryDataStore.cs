using AutocleanManager.Api.Models;

namespace AutocleanManager.Api.Data;

public sealed class InMemoryDataStore
{
    private readonly object _sync = new();

    private int _nextUserId = 4;
    private int _nextVehicleId = 5;
    private int _nextWashTypeId = 4;
    private int _nextAppointmentId = 4;

    public List<User> Users { get; } =
    [
        new User { Id = 1, Name = "Joao Silva", Email = "joao@email.com", Role = "Cliente" },
        new User { Id = 2, Name = "Ana Martins", Email = "ana@email.com", Role = "Cliente" },
        new User { Id = 3, Name = "Carlos Lima", Email = "carlos@email.com", Role = "Funcionario" }
    ];

    public List<Vehicle> Vehicles { get; } =
    [
        new Vehicle { Id = 1, UserId = 1, Brand = "Fiat", Model = "Argo", Plate = "ABC1D23", Color = "Prata", Year = 2021 },
        new Vehicle { Id = 2, UserId = 1, Brand = "Honda", Model = "Civic", Plate = "HIJ7K89", Color = "Preto", Year = 2020 },
        new Vehicle { Id = 3, UserId = 2, Brand = "Volkswagen", Model = "Polo", Plate = "EFG4H56", Color = "Branco", Year = 2022 },
        new Vehicle { Id = 4, UserId = 2, Brand = "Toyota", Model = "Corolla", Plate = "LMN0P12", Color = "Cinza", Year = 2023 }
    ];

    public List<WashType> WashTypes { get; } =
    [
        new WashType { Id = 1, Name = "Lavagem externa", BasePrice = 35m, EstimatedDurationMinutes = 30 },
        new WashType { Id = 2, Name = "Lavagem interna", BasePrice = 40m, EstimatedDurationMinutes = 40 },
        new WashType { Id = 3, Name = "Lavagem completa", BasePrice = 60m, EstimatedDurationMinutes = 60 }
    ];

    public List<Appointment> Appointments { get; } =
    [
        new Appointment
        {
            Id = 1,
            UserId = 1,
            VehicleId = 1,
            WashTypeId = 3,
            DirtLevel = "Pesada",
            ScheduledAt = DateTime.UtcNow.AddHours(4),
            Status = "Confirmado",
            TotalPrice = 72m
        },
        new Appointment
        {
            Id = 2,
            UserId = 1,
            VehicleId = 2,
            WashTypeId = 1,
            DirtLevel = "Leve",
            ScheduledAt = DateTime.UtcNow.AddDays(1),
            Status = "Aguardando",
            TotalPrice = 35m
        },
        new Appointment
        {
            Id = 3,
            UserId = 2,
            VehicleId = 3,
            WashTypeId = 2,
            DirtLevel = "Media",
            ScheduledAt = DateTime.UtcNow.AddDays(2),
            Status = "Na fila",
            TotalPrice = 44m
        }
    ];

    public int NextUserId()
    {
        lock (_sync)
        {
            return _nextUserId++;
        }
    }

    public int NextVehicleId()
    {
        lock (_sync)
        {
            return _nextVehicleId++;
        }
    }

    public int NextWashTypeId()
    {
        lock (_sync)
        {
            return _nextWashTypeId++;
        }
    }

    public int NextAppointmentId()
    {
        lock (_sync)
        {
            return _nextAppointmentId++;
        }
    }
}
