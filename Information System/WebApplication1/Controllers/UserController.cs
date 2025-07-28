using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task AddUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                BadRequest("user cannot be null.");
            }

            await _userService.AddUserAsync(userDto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            userDto.UserId = id;
            await _userService.UpdateUserAsync(id, userDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteUser([FromQuery] Guid guid)
        {
            await _userService.DeleteUserAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid guid)
        {
            var response = await _userService.GetUserByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetAllUsers", Name = "GetAllUsers")]
        public async Task<IActionResult> GetAllUser()
        {
            var response = await _userService.GetAllUsersAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _userService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }
        
        [HttpPost("GetUsername")]
        public async Task<IActionResult> GetUsername([FromBody] LoginDto dto)
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Username == dto.Username);
            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };
            if (userDto == null) return NotFound();
            return Ok(userDto);
        }
    }
}
