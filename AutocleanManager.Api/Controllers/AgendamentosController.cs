using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/agendamentos")]
public sealed class AgendamentosController(ArmazenamentoEmMemoria store) : ControllerBase
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
    public ActionResult<IEnumerable<Agendamento>> GetAll()
    {
        return Ok(store.Appointments);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Agendamento> GetById(int id)
    {
        var Agendamento = store.Appointments.FirstOrDefault(a => a.Id == id);
        if (Agendamento is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        return Ok(Agendamento);
    }

    [HttpPost]
    public ActionResult<Agendamento> Create([FromBody] CriarAgendamentoRequest request)
    {
        var validation = ValidateRequest(
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

        var TipoLavagem = store.WashTypes.First(w => w.Id == request.TipoLavagemId);
        var Agendamento = new Agendamento
        {
            Id = store.NextAppointmentId(),
            UserId = request.UsuarioId,
            VehicleId = request.VeiculoId,
            WashTypeId = request.TipoLavagemId,
            DirtLevel = request.NivelSujeira.Trim(),
            ScheduledAt = request.DataHoraAgendada,
            Status = request.Status.Trim(),
            TotalPrice = CalculadoraPreco.CalculateTotalPrice(TipoLavagem.BasePrice, request.NivelSujeira)
        };

        store.Appointments.Add(Agendamento);
        return CreatedAtAction(nameof(GetById), new { id = Agendamento.Id }, Agendamento);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Agendamento> Update(int id, [FromBody] AtualizarAgendamentoRequest request)
    {
        var Agendamento = store.Appointments.FirstOrDefault(a => a.Id == id);
        if (Agendamento is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        var validation = ValidateRequest(
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

        var TipoLavagem = store.WashTypes.First(w => w.Id == request.TipoLavagemId);

        Agendamento.UserId = request.UsuarioId;
        Agendamento.VehicleId = request.VeiculoId;
        Agendamento.WashTypeId = request.TipoLavagemId;
        Agendamento.DirtLevel = request.NivelSujeira.Trim();
        Agendamento.ScheduledAt = request.DataHoraAgendada;
        Agendamento.Status = request.Status.Trim();
        Agendamento.TotalPrice = CalculadoraPreco.CalculateTotalPrice(TipoLavagem.BasePrice, request.NivelSujeira);

        return Ok(Agendamento);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var Agendamento = store.Appointments.FirstOrDefault(a => a.Id == id);
        if (Agendamento is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        store.Appointments.Remove(Agendamento);
        return NoContent();
    }

    private ActionResult? ValidateRequest(
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

        if (!store.Users.Any(u => u.Id == userId))
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var Veiculo = store.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (Veiculo is null)
        {
            return BadRequest(new { message = "Veiculo informado nao existe." });
        }

        if (Veiculo.UserId != userId)
        {
            return BadRequest(new { message = "Veiculo nao pertence ao usuario informado." });
        }

        if (!store.WashTypes.Any(w => w.Id == washTypeId))
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

        var alreadyBooked = store.Appointments.Any(a =>
            a.Id != currentAppointmentId &&
            a.ScheduledAt == scheduledAt &&
            a.Status != "Cancelado");
        if (alreadyBooked)
        {
            return Conflict(new { message = "Horario ja ocupado." });
        }

        return null;
    }
}
