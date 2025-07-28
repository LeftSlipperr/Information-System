using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userService.AuthenticateAsync(loginDto.Username, loginDto.Password);
        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        return Ok(new { message = "Login successful", user });
    }
}
