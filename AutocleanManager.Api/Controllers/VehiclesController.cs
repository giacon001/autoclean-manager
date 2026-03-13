using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public sealed class VehiclesController(InMemoryDataStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Vehicle>> GetAll()
    {
        return Ok(store.Vehicles);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Vehicle> GetById(int id)
    {
        var vehicle = store.Vehicles.FirstOrDefault(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        return Ok(vehicle);
    }

    [HttpPost]
    public ActionResult<Vehicle> Create([FromBody] CreateVehicleRequest request)
    {
        if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.Brand) || string.IsNullOrWhiteSpace(request.Model) ||
            string.IsNullOrWhiteSpace(request.Plate) || string.IsNullOrWhiteSpace(request.Color))
        {
            return BadRequest(new { message = "UserId, Brand, Model, Plate e Color sao obrigatorios." });
        }

        var userExists = store.Users.Any(u => u.Id == request.UserId);
        if (!userExists)
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var plateInUse = store.Vehicles.Any(v => v.Plate.Equals(request.Plate, StringComparison.OrdinalIgnoreCase));
        if (plateInUse)
        {
            return Conflict(new { message = "Placa ja cadastrada." });
        }

        var vehicle = new Vehicle
        {
            Id = store.NextVehicleId(),
            UserId = request.UserId,
            Brand = request.Brand.Trim(),
            Model = request.Model.Trim(),
            Plate = request.Plate.Trim().ToUpperInvariant(),
            Color = request.Color.Trim(),
            Year = request.Year
        };

        store.Vehicles.Add(vehicle);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Vehicle> Update(int id, [FromBody] UpdateVehicleRequest request)
    {
        var vehicle = store.Vehicles.FirstOrDefault(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.Brand) || string.IsNullOrWhiteSpace(request.Model) ||
            string.IsNullOrWhiteSpace(request.Plate) || string.IsNullOrWhiteSpace(request.Color))
        {
            return BadRequest(new { message = "UserId, Brand, Model, Plate e Color sao obrigatorios." });
        }

        var userExists = store.Users.Any(u => u.Id == request.UserId);
        if (!userExists)
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var plateInUse = store.Vehicles.Any(v => v.Id != id && v.Plate.Equals(request.Plate, StringComparison.OrdinalIgnoreCase));
        if (plateInUse)
        {
            return Conflict(new { message = "Placa ja cadastrada." });
        }

        vehicle.UserId = request.UserId;
        vehicle.Brand = request.Brand.Trim();
        vehicle.Model = request.Model.Trim();
        vehicle.Plate = request.Plate.Trim().ToUpperInvariant();
        vehicle.Color = request.Color.Trim();
        vehicle.Year = request.Year;

        return Ok(vehicle);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var vehicle = store.Vehicles.FirstOrDefault(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        var hasAppointments = store.Appointments.Any(a => a.VehicleId == id);
        if (hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover veiculo com agendamentos vinculados." });
        }

        store.Vehicles.Remove(vehicle);
        return NoContent();
    }
}
