using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradePointEconomicsController : ControllerBase
    {
        private readonly ITradePointEconomicsService _tradePointEconomicsService;
        public TradePointEconomicsController(ITradePointEconomicsService tradePointEconomicsService)
        {
            _tradePointEconomicsService = tradePointEconomicsService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTradePointEconomics([FromBody] TradePointEconomicsDto tradePointEconomicsDto)
        {
            if (tradePointEconomicsDto == null)
                return BadRequest("Объект пустой");

            try
            {
                tradePointEconomicsDto.Profit = tradePointEconomicsDto.Revenue - tradePointEconomicsDto.Expenses;
                await _tradePointEconomicsService.AddTradePointEconomicsAsync(tradePointEconomicsDto);
                return Ok();
            }
            catch (Exception ex)
            {
                // Логируем ошибку (или хотя бы верни текст)
                return StatusCode(500, $"Внутренняя ошибка сервиса: {ex.Message}");
            }
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTradePointEconomics(Guid id, [FromBody] TradePointEconomicsDto tradePointEconomicsDto)
        {
            tradePointEconomicsDto.TradePointEconomicsId = id;
            await _tradePointEconomicsService.UpdateTradePointEconomicsAsync(id, tradePointEconomicsDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteTradePointEconomics([FromQuery] Guid guid)
        {
            await _tradePointEconomicsService.DeleteTradePointEconomicsAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetTradePointEconomicsById")]
        public async Task<IActionResult> GetTradePointEconomicsById([FromRoute] Guid guid)
        {
            var response = await _tradePointEconomicsService.GetTradePointEconomicsByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetTradePointEconomicsByPeriod", Name = "GetTradePointEconomicsByPeriod")]
        public async Task<IActionResult> GetDebtPayableByPeriod([FromQuery] DateTime period)
        {
            var response = await _tradePointEconomicsService.GetTradePointEconomicsByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllTradePointEconomicss", Name = "GetAllTradePointEconomicss")]
        public async Task<IActionResult> GetAllTradePointEconomics()
        {
            var response = await _tradePointEconomicsService.GetAllTradePointEconomicsAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _tradePointEconomicsService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }

    }
}