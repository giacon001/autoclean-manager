using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/wash-types")]
public sealed class WashTypesController(InMemoryDataStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<WashType>> GetAll()
    {
        return Ok(store.WashTypes);
    }

    [HttpGet("{id:int}")]
    public ActionResult<WashType> GetById(int id)
    {
        var washType = store.WashTypes.FirstOrDefault(w => w.Id == id);
        if (washType is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        return Ok(washType);
    }

    [HttpPost]
    public ActionResult<WashType> Create([FromBody] CreateWashTypeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.BasePrice <= 0 || request.EstimatedDurationMinutes <= 0)
        {
            return BadRequest(new { message = "Name, BasePrice e EstimatedDurationMinutes sao obrigatorios e devem ser maiores que zero." });
        }

        var nameInUse = store.WashTypes.Any(w => w.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase));
        if (nameInUse)
        {
            return Conflict(new { message = "Tipo de lavagem ja cadastrado." });
        }

        var washType = new WashType
        {
            Id = store.NextWashTypeId(),
            Name = request.Name.Trim(),
            BasePrice = request.BasePrice,
            EstimatedDurationMinutes = request.EstimatedDurationMinutes
        };

        store.WashTypes.Add(washType);
        return CreatedAtAction(nameof(GetById), new { id = washType.Id }, washType);
    }

    [HttpPut("{id:int}")]
    public ActionResult<WashType> Update(int id, [FromBody] UpdateWashTypeRequest request)
    {
        var washType = store.WashTypes.FirstOrDefault(w => w.Id == id);
        if (washType is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        if (string.IsNullOrWhiteSpace(request.Name) || request.BasePrice <= 0 || request.EstimatedDurationMinutes <= 0)
        {
            return BadRequest(new { message = "Name, BasePrice e EstimatedDurationMinutes sao obrigatorios e devem ser maiores que zero." });
        }

        var nameInUse = store.WashTypes.Any(w => w.Id != id && w.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase));
        if (nameInUse)
        {
            return Conflict(new { message = "Tipo de lavagem ja cadastrado." });
        }

        washType.Name = request.Name.Trim();
        washType.BasePrice = request.BasePrice;
        washType.EstimatedDurationMinutes = request.EstimatedDurationMinutes;

        foreach (var appointment in store.Appointments.Where(a => a.WashTypeId == id))
        {
            appointment.TotalPrice = PriceCalculator.CalculateTotalPrice(washType.BasePrice, appointment.DirtLevel);
        }

        return Ok(washType);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var washType = store.WashTypes.FirstOrDefault(w => w.Id == id);
        if (washType is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        var hasAppointments = store.Appointments.Any(a => a.WashTypeId == id);
        if (hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover tipo de lavagem com agendamentos vinculados." });
        }

        store.WashTypes.Remove(washType);
        return NoContent();
    }
}
