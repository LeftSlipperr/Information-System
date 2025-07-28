using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceAnalysisController : ControllerBase
    {
        private readonly IBalanceAnalysisService _balanceAnalysisService;
        public BalanceAnalysisController(IBalanceAnalysisService balanceAnalysisService)
        {
            _balanceAnalysisService = balanceAnalysisService;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddBalanceAnalysis([FromBody] BalanceAnalysisDto balanceAnalysis)
        {
            if (balanceAnalysis == null)
            {
                return BadRequest("Balance cannot be null.");
            }

            await _balanceAnalysisService.AddIncExpAsync(balanceAnalysis);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBalanceAnalysis(Guid id, [FromBody] BalanceAnalysisDto balanceAnalysisDto)
        {
            balanceAnalysisDto.BalanceId = id;
            await _balanceAnalysisService.UpdateIncExpAsync(id, balanceAnalysisDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteBalanceAnalysis([FromQuery] Guid guid)
        {
            await _balanceAnalysisService.DeleteBalanceAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetBalanceAnalysisById")]
        public async Task<IActionResult> GetBalanceAnalysisByID([FromRoute] Guid guid)
        {
            var response = await _balanceAnalysisService.GetBalanceAnalysisByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetBalanceAnalysisByPeriod", Name = "GetBalanceAnalysisByPeriod")]
        public async Task<IActionResult> GetBalanceAnalysisByPeriod([FromQuery] DateTime period)
        {
            var response = await _balanceAnalysisService.GetBalanceAnalysisByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllBalanceAnalysiss", Name = "GetAllBalanceAnalysiss")]
        public async Task<IActionResult> GetBalanceAnalysis()
        {
            var response = await _balanceAnalysisService.GetAllBalanceAnalysesAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _balanceAnalysisService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }

    }
}
