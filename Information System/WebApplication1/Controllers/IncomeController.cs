using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;
        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        [HttpPost(Name = "AddIncome")]
        public async Task AddIncome([FromBody] IncomeDto incomeDto)
        {
            if (incomeDto == null)
            {
                BadRequest("balance cannot be null.");
            }

            await _incomeService.AddIncomeAsync(incomeDto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateIncome(Guid id, [FromBody] IncomeDto incomeDto)
        {
            incomeDto.IncomeId = id;
            await _incomeService.UpdateIncomeAsync(id, incomeDto);
            return Ok();
        }

        [HttpDelete(Name = "DeleteIncome")]
        public async Task DeleteIncome([FromQuery] Guid guid)
        {
            await _incomeService.DeleteIncomeAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetIncomeById")]
        public async Task<IActionResult> GetIncomeById([FromRoute] Guid guid)
        {
            var response = await _incomeService.GetIncomeByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetIncomeByPeriod", Name = "GetIncomeByPeriod")]
        public async Task<IActionResult> GetIncomeByPeriod([FromQuery] DateTime period)
        {
            var response = await _incomeService.GetIncomeByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllIncomes", Name = "GetAllIncomes")]
        public async Task<IActionResult> GetAllIncomes()
        {
            var response = await _incomeService.GetAllIncomesAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _incomeService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }

    }
}
