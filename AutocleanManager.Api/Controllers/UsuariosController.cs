using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public sealed class UsuariosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
    {
        var usuarios = await db.Usuarios.ToListAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Create([FromBody] CriarUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Nome e email sao obrigatorios." });
        }

        var normalizedEmail = request.Email.Trim().ToLower();
        var emailInUse = await db.Usuarios.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailInUse)
        {
            return Conflict(new { message = "Email ja cadastrado." });
        }

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = request.Email.Trim(),
            Papel = string.IsNullOrWhiteSpace(request.Papel) ? "Cliente" : request.Papel.Trim()
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Usuario>> Update(int id, [FromBody] AtualizarUsuarioRequest request)
    {
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Nome e email sao obrigatorios." });
        }

        var normalizedEmail = request.Email.Trim().ToLower();
        var emailInUse = await db.Usuarios.AnyAsync(u => u.Id != id && u.Email.ToLower() == normalizedEmail);
        if (emailInUse)
        {
            return Conflict(new { message = "Email ja cadastrado." });
        }

        usuario.Nome = request.Nome.Trim();
        usuario.Email = request.Email.Trim();
        if (!string.IsNullOrWhiteSpace(request.Papel))
        {
            usuario.Papel = request.Papel.Trim();
        }

        db.Usuarios.Update(usuario);
        await db.SaveChangesAsync();
        return Ok(usuario);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        var hasVehicles = await db.Veiculos.AnyAsync(v => v.UsuarioId == id);
        var hasAppointments = await db.Agendamentos.AnyAsync(a => a.UsuarioId == id);
        if (hasVehicles || hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover usuario com veiculos ou agendamentos vinculados." });
        }

        db.Usuarios.Remove(usuario);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
