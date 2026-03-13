using AutocleanManager.Api.Data;
using AutocleanManager.Api.Models;
using AutocleanManager.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AutocleanManager.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(InMemoryDataStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll()
    {
        return Ok(store.Users);
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = store.Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        return Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Name e Email sao obrigatorios." });
        }

        var emailInUse = store.Users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        if (emailInUse)
        {
            return Conflict(new { message = "Email ja cadastrado." });
        }

        var user = new User
        {
            Id = store.NextUserId(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Cliente" : request.Role.Trim()
        };

        store.Users.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public ActionResult<User> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var user = store.Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Name e Email sao obrigatorios." });
        }

        var emailInUse = store.Users.Any(u => u.Id != id && u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        if (emailInUse)
        {
            return Conflict(new { message = "Email ja cadastrado." });
        }

        user.Name = request.Name.Trim();
        user.Email = request.Email.Trim();
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            user.Role = request.Role.Trim();
        }

        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var user = store.Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound(new { message = "Usuario nao encontrado." });
        }

        var hasVehicles = store.Vehicles.Any(v => v.UserId == id);
        var hasAppointments = store.Appointments.Any(a => a.UserId == id);
        if (hasVehicles || hasAppointments)
        {
            return Conflict(new { message = "Nao e possivel remover usuario com veiculos ou agendamentos vinculados." });
        }

        store.Users.Remove(user);
        return NoContent();
    }
}
