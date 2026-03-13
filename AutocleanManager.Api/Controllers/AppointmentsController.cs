using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public sealed class AppointmentsController(InMemoryDataStore store) : ControllerBase
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
    public ActionResult<IEnumerable<Appointment>> GetAll()
    {
        return Ok(store.Appointments);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Appointment> GetById(int id)
    {
        var appointment = store.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        return Ok(appointment);
    }

    [HttpPost]
    public ActionResult<Appointment> Create([FromBody] CreateAppointmentRequest request)
    {
        var validation = ValidateRequest(
            request.UserId,
            request.VehicleId,
            request.WashTypeId,
            request.DirtLevel,
            request.Status,
            request.ScheduledAt,
            null);
        if (validation is not null)
        {
            return validation;
        }

        var washType = store.WashTypes.First(w => w.Id == request.WashTypeId);
        var appointment = new Appointment
        {
            Id = store.NextAppointmentId(),
            UserId = request.UserId,
            VehicleId = request.VehicleId,
            WashTypeId = request.WashTypeId,
            DirtLevel = request.DirtLevel.Trim(),
            ScheduledAt = request.ScheduledAt,
            Status = request.Status.Trim(),
            TotalPrice = PriceCalculator.CalculateTotalPrice(washType.BasePrice, request.DirtLevel)
        };

        store.Appointments.Add(appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Appointment> Update(int id, [FromBody] UpdateAppointmentRequest request)
    {
        var appointment = store.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        var validation = ValidateRequest(
            request.UserId,
            request.VehicleId,
            request.WashTypeId,
            request.DirtLevel,
            request.Status,
            request.ScheduledAt,
            id);
        if (validation is not null)
        {
            return validation;
        }

        var washType = store.WashTypes.First(w => w.Id == request.WashTypeId);

        appointment.UserId = request.UserId;
        appointment.VehicleId = request.VehicleId;
        appointment.WashTypeId = request.WashTypeId;
        appointment.DirtLevel = request.DirtLevel.Trim();
        appointment.ScheduledAt = request.ScheduledAt;
        appointment.Status = request.Status.Trim();
        appointment.TotalPrice = PriceCalculator.CalculateTotalPrice(washType.BasePrice, request.DirtLevel);

        return Ok(appointment);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var appointment = store.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment is null)
        {
            return NotFound(new { message = "Agendamento nao encontrado." });
        }

        store.Appointments.Remove(appointment);
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
            return BadRequest(new { message = "UserId, VehicleId e WashTypeId sao obrigatorios." });
        }

        if (!store.Users.Any(u => u.Id == userId))
        {
            return BadRequest(new { message = "Usuario informado nao existe." });
        }

        var vehicle = store.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (vehicle is null)
        {
            return BadRequest(new { message = "Veiculo informado nao existe." });
        }

        if (vehicle.UserId != userId)
        {
            return BadRequest(new { message = "Veiculo nao pertence ao usuario informado." });
        }

        if (!store.WashTypes.Any(w => w.Id == washTypeId))
        {
            return BadRequest(new { message = "Tipo de lavagem informado nao existe." });
        }

        if (string.IsNullOrWhiteSpace(dirtLevel) || !AllowedDirtLevels.Contains(dirtLevel.Trim()))
        {
            return BadRequest(new { message = "DirtLevel invalido. Use: Leve, Media ou Pesada." });
        }

        if (string.IsNullOrWhiteSpace(status) || !AllowedStatuses.Contains(status.Trim()))
        {
            return BadRequest(new { message = "Status invalido." });
        }

        if (scheduledAt == default)
        {
            return BadRequest(new { message = "ScheduledAt e obrigatorio." });
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
