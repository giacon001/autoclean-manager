using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/tipos-lavagem")]
public sealed class TiposLavagemController(ArmazenamentoEmMemoria store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<TipoLavagem>> GetAll()
    {
        return Ok(store.WashTypes);
    }

    [HttpGet("{id:int}")]
    public ActionResult<TipoLavagem> GetById(int id)
    {
        var TipoLavagem = store.WashTypes.FirstOrDefault(w => w.Id == id);
        if (TipoLavagem is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        return Ok(TipoLavagem);
    }

    [HttpPost]
    public ActionResult<TipoLavagem> Create([FromBody] CriarTipoLavagemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || request.PrecoBase <= 0 || request.DuracaoEstimadaMinutos <= 0)
        {
            return BadRequest(new { message = "Nome, preco base e duracao estimada em minutos sao obrigatorios e devem ser maiores que zero." });
        }

        var nameInUse = store.WashTypes.Any(w => w.Name.Equals(request.Nome, StringComparison.OrdinalIgnoreCase));
        if (nameInUse)
        {
            return Conflict(new { message = "Tipo de lavagem ja cadastrado." });
        }

        var TipoLavagem = new TipoLavagem
        {
            Id = store.NextWashTypeId(),
            Name = request.Nome.Trim(),
            BasePrice = request.PrecoBase,
            EstimatedDurationMinutes = request.DuracaoEstimadaMinutos
        };

        store.WashTypes.Add(TipoLavagem);
        return CreatedAtAction(nameof(GetById), new { id = TipoLavagem.Id }, TipoLavagem);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TipoLavagem> Update(int id, [FromBody] AtualizarTipoLavagemRequest request)
    {
        var TipoLavagem = store.WashTypes.FirstOrDefault(w => w.Id == id);
        if (TipoLavagem is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        if (string.IsNullOrWhiteSpace(request.Nome) || request.PrecoBase <= 0 || request.DuracaoEstimadaMinutos <= 0)
        {
            return BadRequest(new { message = "Nome, preco base e duracao estimada em minutos sao obrigatorios e devem ser maiores que zero." });
        }

        var nameInUse = store.WashTypes.Any(w => w.Id != id && w.Name.Equals(request.Nome, StringComparison.OrdinalIgnoreCase));
        if (nameInUse)
        {
            return Conflict(new { message = "Tipo de lavagem ja cadastrado." });
        }

        TipoLavagem.Name = request.Nome.Trim();
        TipoLavagem.BasePrice = request.PrecoBase;
        TipoLavagem.EstimatedDurationMinutes = request.DuracaoEstimadaMinutos;

        foreach (var Agendamento in store.Appointments.Where(a => a.WashTypeId == id))
        {
            Agendamento.TotalPrice = CalculadoraPreco.CalculateTotalPrice(TipoLavagem.BasePrice, Agendamento.DirtLevel);
        }

        return Ok(TipoLavagem);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var TipoLavagem = store.WashTypes.FirstOrDefault(w => w.Id == id);
        if (TipoLavagem is null)
        {
            return NotFound(new { message = "Tipo de lavagem nao encontrado." });
        }

        var hasAppointments = store.Appointments.Any(a => a.WashTypeId == id);
        if (hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover tipo de lavagem com agendamentos vinculados." });
        }

        store.WashTypes.Remove(TipoLavagem);
        return NoContent();
    }
}
