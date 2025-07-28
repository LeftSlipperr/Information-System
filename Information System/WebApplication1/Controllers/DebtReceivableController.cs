using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebtReceivableController : ControllerBase
    {
        private readonly IDebtReceivableService _debtReceivableService;
        public DebtReceivableController(IDebtReceivableService debtReceivableService)
        {
            _debtReceivableService = debtReceivableService;
        }

        [HttpPost]
        public async Task AddDebtReceivable([FromBody] DebtReceivableDto debtReceivableDto)
        {
            if (debtReceivableDto == null)
            {
                BadRequest("balance cannot be null.");
            }

            await _debtReceivableService.AddDebt(debtReceivableDto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDebtReceivable(Guid id, [FromBody] DebtReceivableDto debtReceivableDto)
        {
            debtReceivableDto.DebtReceivableId = id;
            await _debtReceivableService.UpdateDebtAsync(id, debtReceivableDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteDebtReceivable([FromQuery] Guid guid)
        {
            await _debtReceivableService.DeleteDebtAsync(guid);
        }

        [HttpGet("{guid}", Name = "DetDebtReceivableById")]
        public async Task<IActionResult> GetDebtReceivableById([FromRoute] Guid guid)
        {
            var response = await _debtReceivableService.GetDebtByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetDebtReceivableByPeriod", Name = "GetDebtReceivableByPeriod")]
        public async Task<IActionResult> GetDebtReceivableByPeriod([FromQuery] DateTime period)
        {
            var response = await _debtReceivableService.GetDebtByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllDebtReceivables", Name = "GetAllDebtReceivables")]
        public async Task<IActionResult> GetAllDebtReceivable()
        {
            var response = await _debtReceivableService.GetAllDebtsAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _debtReceivableService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }

    }
}
