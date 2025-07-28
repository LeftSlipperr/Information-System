using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;
        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet("GetAllLogs")]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _logService.GetLogsAsync();
            return Ok(logs);
        }

        [HttpPost("AddLog")]
        public async Task<IActionResult> AddLog([FromBody] LogDto logDto)
        {
            if (logDto == null)
            {
                return BadRequest("Log cannot be null.");
            }

            await _logService.AddLogAsync(logDto);
            return Ok();
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _logService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }
    }
}
