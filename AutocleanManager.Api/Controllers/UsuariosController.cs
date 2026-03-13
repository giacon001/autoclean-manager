using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public sealed class UsuariosController(ArmazenamentoEmMemoria store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Usuario>> GetAll()
    {
        return Ok(store.Users);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Usuario> GetById(int id)
    {
        var Usuario = store.Users.FirstOrDefault(u => u.Id == id);
        if (Usuario is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        return Ok(Usuario);
    }

    [HttpPost]
    public ActionResult<Usuario> Create([FromBody] CriarUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Nome e email sao obrigatorios." });
        }

        var emailInUse = store.Users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        if (emailInUse)
        {
            return Conflict(new { message = "Email ja cadastrado." });
        }

        var Usuario = new Usuario
        {
            Id = store.NextUserId(),
            Name = request.Nome.Trim(),
            Email = request.Email.Trim(),
            Role = string.IsNullOrWhiteSpace(request.Papel) ? "Cliente" : request.Papel.Trim()
        };

        store.Users.Add(Usuario);
        return CreatedAtAction(nameof(GetById), new { id = Usuario.Id }, Usuario);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Usuario> Update(int id, [FromBody] AtualizarUsuarioRequest request)
    {
        var Usuario = store.Users.FirstOrDefault(u => u.Id == id);
        if (Usuario is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Nome e email sao obrigatorios." });
        }

        var emailInUse = store.Users.Any(u => u.Id != id && u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        if (emailInUse)
        {
            return Conflict(new { message = "Email ja cadastrado." });
        }

        Usuario.Name = request.Nome.Trim();
        Usuario.Email = request.Email.Trim();
        if (!string.IsNullOrWhiteSpace(request.Papel))
        {
            Usuario.Role = request.Papel.Trim();
        }

        return Ok(Usuario);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var Usuario = store.Users.FirstOrDefault(u => u.Id == id);
        if (Usuario is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        var hasVehicles = store.Vehicles.Any(v => v.UserId == id);
        var hasAppointments = store.Appointments.Any(a => a.UserId == id);
        if (hasVehicles || hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover usuario com veiculos ou agendamentos vinculados." });
        }

        store.Users.Remove(Usuario);
        return NoContent();
    }
}
