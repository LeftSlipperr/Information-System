using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DebtPayableController : ControllerBase
    {
        private readonly IDebtPayableService _debtPayableService;
        public DebtPayableController(IDebtPayableService debtPayableService)
        {
            _debtPayableService = debtPayableService;
        }

        [HttpPost]
        public async Task AddDebtPayable([FromBody] DebtPayableDto debtPayableDto)
        {
            Console.WriteLine("Получен запрос на добавление: " + DateTime.Now); 
            if (debtPayableDto == null)
            {
                BadRequest("balance cannot be null.");
            }

            await _debtPayableService.AddDebtAsync(debtPayableDto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDebtPayable(Guid id, [FromBody] DebtPayableDto debtPayableDto)
        {
            debtPayableDto.DebtPayableId = id;
            await _debtPayableService.UpdateDebtAsync(id, debtPayableDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteDebtPayable([FromQuery] Guid guid)
        {
            await _debtPayableService.DeleteDebtAsync(guid);
        }

        [HttpGet("{guid}", Name = "DebtPayableById")]
        public async Task<IActionResult> GetDebtPayableById([FromRoute] Guid guid)
        {
            var response = await _debtPayableService.GetDebtByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetDebtPayablesByPeriod", Name = "GetDebtPayablesByPeriod")]
        public async Task<IActionResult> GetDebtPayableByPeriod([FromQuery] DateTime period)
        {
            var response = await _debtPayableService.GetDebtByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllDebtPayables", Name = "GetAllDebtPayables")]
        public async Task<IActionResult> GetAllDebtPayables()
        {
            var response = await _debtPayableService.GetAllDebtsAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _debtPayableService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }

    }
}
