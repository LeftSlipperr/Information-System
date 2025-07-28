using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;
        public SalaryController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddSalary([FromBody] SalaryDto salaryDto)
        {
            return BadRequest("Direct addition of salaries is not allowed.");

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSalary(Guid id, [FromBody] SalaryDto salaryDto)
        {
            salaryDto.SalaryId = id;

            await _salaryService.UpdateSalaryAsync(id, salaryDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteSalary([FromQuery] Guid guid)
        {
            await _salaryService.DeleteSalaryAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetSalaryById")]
        public async Task<IActionResult> GetSalaryById([FromRoute] Guid guid)
        {
            var response = await _salaryService.GetSalaryByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetSalaryByPeriod", Name = "GetSalaryByPeriod")]
        public async Task<IActionResult> GetSalaryByPeriod([FromQuery] DateTime period)
        {
            var response = await _salaryService.GetSalaryByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllSalarys", Name = "GetAllSalarys")]
        public async Task<IActionResult> GetAllSalary()
        {
            var response = await _salaryService.GetAllSalariesAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _salaryService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }
    }
}
