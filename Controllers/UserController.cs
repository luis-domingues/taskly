using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Taskly.DTOs;
using Taskly.Exceptions;
using Taskly.Models;
using Taskly.Services;

namespace Taskly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] User user)
    {
        try
        {
            var createdUser = await _userService.RegisterUserAsync(user);
            return CreatedAtAction(nameof(RegisterUser), new { id = createdUser.Id }, createdUser);
        }
        catch (UsernameAlreadyExistsException e)
        {
            return Conflict(new { error = e.Message });
        }
        catch (EmailAlreadyExistsException e)
        {
            return Conflict(new { error = e.Message });
        }
        catch (PasswordAlreadyExistsException e)
        {
            return Conflict(new { error = e.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Error ao processar a requisição do cliente." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto request)
    {
        try
        {
            var user = await _userService.LoginAsync(request.Username, request.Password);
            return Ok(new { message = "Login bem-sucedido.", user });
        }
        catch (UsernameNull e)
        {
            return NotFound(new { error = e.Message });
        }
        catch (IncorrectPassword e)
        {
            return Unauthorized(new { error = e.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Requisisão inválida." });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            var updatedUser = new User
            {
                FullName = request.FullName,
                Username = request.Username,
                Password = request.Password,
                TitleJob = request.TitleJob
            };

            var user = await _userService.UpdateUserAsync(id, updatedUser);
            return Ok(new { message = "Informações atualizadas com sucesso." });
        }
        catch (UsernameAlreadyExistsException e)
        {
            return Conflict(new { error = e.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var authenticatedUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (id != authenticatedUserId)
                return Unauthorized(new { error = "Acesso não permitido." });

            await _userService.DeleteUserAsync(id);

            return Ok(new { message = "Usuário deletado com sucesso." });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string? fullname, [FromQuery] string? username,
        [FromQuery] string? email)
    {
        try
        {
            var users = await _userService.SearchUserAsync(fullname, username, email);

            if (users.Count == 0)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }
}