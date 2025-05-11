using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuth _authService;

    public AuthController(IAuth authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var (success, message) = await _authService.SignupAsync(model);
        return success ? Ok(message) : BadRequest(message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var (success, tokenOrMessage) = await _authService.LoginAsync(model.Email, model.Password);
        return success ? Ok(new { token = tokenOrMessage }) : Unauthorized(tokenOrMessage);
    }
}
