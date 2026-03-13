using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/veiculos")]
public sealed class VeiculosController(ArmazenamentoEmMemoria store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Veiculo>> GetAll()
    {
        return Ok(store.Vehicles);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Veiculo> GetById(int id)
    {
        var Veiculo = store.Vehicles.FirstOrDefault(v => v.Id == id);
        if (Veiculo is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        return Ok(Veiculo);
    }

    [HttpPost]
    public ActionResult<Veiculo> Create([FromBody] CriarVeiculoRequest request)
    {
        if (request.UsuarioId <= 0 || string.IsNullOrWhiteSpace(request.Marca) || string.IsNullOrWhiteSpace(request.Modelo) ||
            string.IsNullOrWhiteSpace(request.Placa) || string.IsNullOrWhiteSpace(request.Cor))
        {
            return BadRequest(new { message = "UsuarioId, marca, modelo, placa e cor sao obrigatorios." });
        }

        var userExists = store.Users.Any(u => u.Id == request.UsuarioId);
        if (!userExists)
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var plateInUse = store.Vehicles.Any(v => v.Plate.Equals(request.Placa, StringComparison.OrdinalIgnoreCase));
        if (plateInUse)
        {
            return Conflict(new { message = "Placa ja cadastrada." });
        }

        var Veiculo = new Veiculo
        {
            Id = store.NextVehicleId(),
            UserId = request.UsuarioId,
            Brand = request.Marca.Trim(),
            Model = request.Modelo.Trim(),
            Plate = request.Placa.Trim().ToUpperInvariant(),
            Color = request.Cor.Trim(),
            Year = request.Ano
        };

        store.Vehicles.Add(Veiculo);
        return CreatedAtAction(nameof(GetById), new { id = Veiculo.Id }, Veiculo);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Veiculo> Update(int id, [FromBody] AtualizarVeiculoRequest request)
    {
        var Veiculo = store.Vehicles.FirstOrDefault(v => v.Id == id);
        if (Veiculo is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        if (request.UsuarioId <= 0 || string.IsNullOrWhiteSpace(request.Marca) || string.IsNullOrWhiteSpace(request.Modelo) ||
            string.IsNullOrWhiteSpace(request.Placa) || string.IsNullOrWhiteSpace(request.Cor))
        {
            return BadRequest(new { message = "UsuarioId, marca, modelo, placa e cor sao obrigatorios." });
        }

        var userExists = store.Users.Any(u => u.Id == request.UsuarioId);
        if (!userExists)
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var plateInUse = store.Vehicles.Any(v => v.Id != id && v.Plate.Equals(request.Placa, StringComparison.OrdinalIgnoreCase));
        if (plateInUse)
        {
            return Conflict(new { message = "Placa ja cadastrada." });
        }

        Veiculo.UserId = request.UsuarioId;
        Veiculo.Brand = request.Marca.Trim();
        Veiculo.Model = request.Modelo.Trim();
        Veiculo.Plate = request.Placa.Trim().ToUpperInvariant();
        Veiculo.Color = request.Cor.Trim();
        Veiculo.Year = request.Ano;

        return Ok(Veiculo);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var Veiculo = store.Vehicles.FirstOrDefault(v => v.Id == id);
        if (Veiculo is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        var hasAppointments = store.Appointments.Any(a => a.VehicleId == id);
        if (hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover veiculo com agendamentos vinculados." });
        }

        store.Vehicles.Remove(Veiculo);
        return NoContent();
    }
}
