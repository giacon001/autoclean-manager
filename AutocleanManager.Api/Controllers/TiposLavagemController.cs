using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/tipos-lavagem")]
public sealed class TiposLavagemController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoLavagem>>> GetAll()
    {
        var tipos = await db.TiposLavagem.ToListAsync();
        return Ok(tipos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TipoLavagem>> GetById(int id)
    {
        var tipoLavagem = await db.TiposLavagem.FirstOrDefaultAsync(w => w.Id == id);
        if (tipoLavagem is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        return Ok(tipoLavagem);
    }

    [HttpPost]
    public async Task<ActionResult<TipoLavagem>> Create([FromBody] CriarTipoLavagemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || request.PrecoBase <= 0 || request.DuracaoEstimadaMinutos <= 0)
        {
            return BadRequest(new { message = "Nome, preco base e duracao estimada em minutos sao obrigatorios e devem ser maiores que zero." });
        }

        var normalizedName = request.Nome.Trim().ToLower();
        var nameInUse = await db.TiposLavagem.AnyAsync(w => w.Nome.ToLower() == normalizedName);
        if (nameInUse)
        {
            return Conflict(new { message = "Tipo de lavagem ja cadastrado." });
        }

        var tipoLavagem = new TipoLavagem
        {
            Nome = request.Nome.Trim(),
            PrecoBase = request.PrecoBase,
            DuracaoEstimadaMinutos = request.DuracaoEstimadaMinutos
        };

        db.TiposLavagem.Add(tipoLavagem);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = tipoLavagem.Id }, tipoLavagem);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TipoLavagem>> Update(int id, [FromBody] AtualizarTipoLavagemRequest request)
    {
        var tipoLavagem = await db.TiposLavagem.FirstOrDefaultAsync(w => w.Id == id);
        if (tipoLavagem is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        if (string.IsNullOrWhiteSpace(request.Nome) || request.PrecoBase <= 0 || request.DuracaoEstimadaMinutos <= 0)
        {
            return BadRequest(new { message = "Nome, preco base e duracao estimada em minutos sao obrigatorios e devem ser maiores que zero." });
        }

        var normalizedName = request.Nome.Trim().ToLower();
        var nameInUse = await db.TiposLavagem.AnyAsync(w => w.Id != id && w.Nome.ToLower() == normalizedName);
        if (nameInUse)
        {
            return Conflict(new { message = "Tipo de lavagem ja cadastrado." });
        }

        tipoLavagem.Nome = request.Nome.Trim();
        tipoLavagem.PrecoBase = request.PrecoBase;
        tipoLavagem.DuracaoEstimadaMinutos = request.DuracaoEstimadaMinutos;

        var agendamentos = await db.Agendamentos.Where(a => a.TipoLavagemId == id).ToListAsync();
        foreach (var agendamento in agendamentos)
        {
            agendamento.PrecoTotal = CalculadoraPreco.CalculateTotalPrice(tipoLavagem.PrecoBase, agendamento.NivelSujeira);
        }

        db.TiposLavagem.Update(tipoLavagem);
        db.Agendamentos.UpdateRange(agendamentos);
        await db.SaveChangesAsync();
        return Ok(tipoLavagem);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tipoLavagem = await db.TiposLavagem.FirstOrDefaultAsync(w => w.Id == id);
        if (tipoLavagem is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        var hasAppointments = await db.Agendamentos.AnyAsync(a => a.TipoLavagemId == id);
        if (hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover tipo de lavagem com agendamentos vinculados." });
        }

        db.TiposLavagem.Remove(tipoLavagem);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
