using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/veiculos")]
public sealed class VeiculosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Veiculo>>> GetAll()
    {
        var veiculos = await db.Veiculos.ToListAsync();
        return Ok(veiculos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Veiculo>> GetById(int id)
    {
        var veiculo = await db.Veiculos.FirstOrDefaultAsync(v => v.Id == id);
        if (veiculo is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        return Ok(veiculo);
    }

    [HttpPost]
    public async Task<ActionResult<Veiculo>> Create([FromBody] CriarVeiculoRequest request)
    {
        if (request.UsuarioId <= 0 || string.IsNullOrWhiteSpace(request.Marca) || string.IsNullOrWhiteSpace(request.Modelo) ||
            string.IsNullOrWhiteSpace(request.Placa) || string.IsNullOrWhiteSpace(request.Cor))
        {
            return BadRequest(new { message = "UsuarioId, marca, modelo, placa e cor sao obrigatorios." });
        }

        var userExists = await db.Usuarios.AnyAsync(u => u.Id == request.UsuarioId);
        if (!userExists)
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var normalizedPlate = request.Placa.Trim().ToUpper();
        var plateInUse = await db.Veiculos.AnyAsync(v => v.Placa.ToUpper() == normalizedPlate);
        if (plateInUse)
        {
            return Conflict(new { message = "Placa ja cadastrada." });
        }

        var veiculo = new Veiculo
        {
            UsuarioId = request.UsuarioId,
            Marca = request.Marca.Trim(),
            Modelo = request.Modelo.Trim(),
            Placa = request.Placa.Trim().ToUpperInvariant(),
            Cor = request.Cor.Trim(),
            Ano = request.Ano
        };

        db.Veiculos.Add(veiculo);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, veiculo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Veiculo>> Update(int id, [FromBody] AtualizarVeiculoRequest request)
    {
        var veiculo = await db.Veiculos.FirstOrDefaultAsync(v => v.Id == id);
        if (veiculo is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        if (request.UsuarioId <= 0 || string.IsNullOrWhiteSpace(request.Marca) || string.IsNullOrWhiteSpace(request.Modelo) ||
            string.IsNullOrWhiteSpace(request.Placa) || string.IsNullOrWhiteSpace(request.Cor))
        {
            return BadRequest(new { message = "UsuarioId, marca, modelo, placa e cor sao obrigatorios." });
        }

        var userExists = await db.Usuarios.AnyAsync(u => u.Id == request.UsuarioId);
        if (!userExists)
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var normalizedPlate = request.Placa.Trim().ToUpper();
        var plateInUse = await db.Veiculos.AnyAsync(v => v.Id != id && v.Placa.ToUpper() == normalizedPlate);
        if (plateInUse)
        {
            return Conflict(new { message = "Placa ja cadastrada." });
        }

        veiculo.UsuarioId = request.UsuarioId;
        veiculo.Marca = request.Marca.Trim();
        veiculo.Modelo = request.Modelo.Trim();
        veiculo.Placa = request.Placa.Trim().ToUpperInvariant();
        veiculo.Cor = request.Cor.Trim();
        veiculo.Ano = request.Ano;

        db.Veiculos.Update(veiculo);
        await db.SaveChangesAsync();
        return Ok(veiculo);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var veiculo = await db.Veiculos.FirstOrDefaultAsync(v => v.Id == id);
        if (veiculo is null)
        {
            return NotFound(new { message = "Veiculo nao encontrado." });
        }

        var hasAppointments = await db.Agendamentos.AnyAsync(a => a.VeiculoId == id);
        if (hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover veiculo com agendamentos vinculados." });
        }

        db.Veiculos.Remove(veiculo);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
