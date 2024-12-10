using Microsoft.AspNetCore.Identity.Data;
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
}