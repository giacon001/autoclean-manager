using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/agendamentos")]
public sealed class AgendamentosController(AppDbContext db) : ControllerBase
{
    private static readonly HashSet<string> AllowedDirtLevels =
    [
        "Leve",
        "Media",
        "Pesada"
    ];

    private static readonly HashSet<string> AllowedStatuses =
    [
        "Aguardando",
        "Confirmado",
        "Na fila",
        "Lavando",
        "Pronto para retirada",
        "Finalizado",
        "Cancelado"
    ];

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetAll()
    {
        var agendamentos = await db.Agendamentos.ToListAsync();
        return Ok(agendamentos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Agendamento>> GetById(int id)
    {
        var agendamento = await db.Agendamentos.FirstOrDefaultAsync(a => a.Id == id);
        if (agendamento is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        return Ok(agendamento);
    }

    [HttpPost]
    public async Task<ActionResult<Agendamento>> Create([FromBody] CriarAgendamentoRequest request)
    {
        var validation = await ValidateRequest(
            request.UsuarioId,
            request.VeiculoId,
            request.TipoLavagemId,
            request.NivelSujeira,
            request.Status,
            request.DataHoraAgendada,
            null);
        if (validation is not null)
        {
            return validation;
        }

        var tipoLavagem = await db.TiposLavagem.FirstAsync(w => w.Id == request.TipoLavagemId);
        var agendamento = new Agendamento
        {
            UsuarioId = request.UsuarioId,
            VeiculoId = request.VeiculoId,
            TipoLavagemId = request.TipoLavagemId,
            NivelSujeira = request.NivelSujeira.Trim(),
            DataHoraAgendada = request.DataHoraAgendada,
            Status = request.Status.Trim(),
            PrecoTotal = CalculadoraPreco.CalculateTotalPrice(tipoLavagem.PrecoBase, request.NivelSujeira)
        };

        db.Agendamentos.Add(agendamento);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = agendamento.Id }, agendamento);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Agendamento>> Update(int id, [FromBody] AtualizarAgendamentoRequest request)
    {
        var agendamento = await db.Agendamentos.FirstOrDefaultAsync(a => a.Id == id);
        if (agendamento is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        var validation = await ValidateRequest(
            request.UsuarioId,
            request.VeiculoId,
            request.TipoLavagemId,
            request.NivelSujeira,
            request.Status,
            request.DataHoraAgendada,
            id);
        if (validation is not null)
        {
            return validation;
        }

        var tipoLavagem = await db.TiposLavagem.FirstAsync(w => w.Id == request.TipoLavagemId);

        agendamento.UsuarioId = request.UsuarioId;
        agendamento.VeiculoId = request.VeiculoId;
        agendamento.TipoLavagemId = request.TipoLavagemId;
        agendamento.NivelSujeira = request.NivelSujeira.Trim();
        agendamento.DataHoraAgendada = request.DataHoraAgendada;
        agendamento.Status = request.Status.Trim();
        agendamento.PrecoTotal = CalculadoraPreco.CalculateTotalPrice(tipoLavagem.PrecoBase, request.NivelSujeira);

        db.Agendamentos.Update(agendamento);
        await db.SaveChangesAsync();
        return Ok(agendamento);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var agendamento = await db.Agendamentos.FirstOrDefaultAsync(a => a.Id == id);
        if (agendamento is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        db.Agendamentos.Remove(agendamento);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidateRequest(
        int userId,
        int vehicleId,
        int washTypeId,
        string dirtLevel,
        string status,
        DateTime scheduledAt,
        int? currentAppointmentId)
    {
        if (userId <= 0 || vehicleId <= 0 || washTypeId <= 0)
        {
            return BadRequest(new { message = "UsuarioId, veiculoId e tipoLavagemId sao obrigatorios." });
        }

        if (!await db.Usuarios.AnyAsync(u => u.Id == userId))
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var veiculo = await db.Veiculos.FirstOrDefaultAsync(v => v.Id == vehicleId);
        if (veiculo is null)
        {
            return BadRequest(new { message = "Veiculo informado nao existe." });
        }

        if (veiculo.UsuarioId != userId)
        {
            return BadRequest(new { message = "Veiculo nao pertence ao usuario informado." });
        }

        if (!await db.TiposLavagem.AnyAsync(w => w.Id == washTypeId))
        {
            return BadRequest(new { message = "Tipo de lavagem informado nao existe." });
        }

        if (string.IsNullOrWhiteSpace(dirtLevel) || !AllowedDirtLevels.Contains(dirtLevel.Trim()))
        {
            return BadRequest(new { message = "Nivel de sujeira invalido. Use: Leve, Media ou Pesada." });
        }

        if (string.IsNullOrWhiteSpace(status) || !AllowedStatuses.Contains(status.Trim()))
        {
            return BadRequest(new { message = "Status invalido." });
        }

        if (scheduledAt == default)
        {
            return BadRequest(new { message = "Data e horario do agendamento sao obrigatorios." });
        }

        var alreadyBooked = await db.Agendamentos.AnyAsync(a =>
            a.Id != currentAppointmentId &&
            a.DataHoraAgendada == scheduledAt &&
            a.Status != "Cancelado");
        if (alreadyBooked)
        {
            return Conflict(new { message = "Horario ja ocupado." });
        }

        return null;
    }
}
