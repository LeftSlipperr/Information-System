using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradePointController : ControllerBase
    {
        private readonly ITradePointService _tradePointService;
        public TradePointController(ITradePointService tradePointService)
        {
            _tradePointService = tradePointService;
        }

        [HttpPost]
        public async Task AddTradePoint([FromBody] TradePointDto tradePointDto)
        {
            if (tradePointDto == null)
            {
                BadRequest("balance cannot be null.");
            }

            await _tradePointService.AddTradePointAsync(tradePointDto);

        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTradePoint(Guid id, [FromBody] TradePointDto tradePointDto)
        {
            tradePointDto.TradePointId = id;
            if (id != tradePointDto.TradePointId)
                return BadRequest("Id mismatch");

            await _tradePointService.UpdateTradePointAsync(id, tradePointDto);
            return Ok();
        }

        /*[HttpPut("{id}")]
        public async Task<IActionResult> PutTradePoint([FromBody] TradePointDto tradePointDto)
        {
            if (id != tradePointDto.TradePointId)
                return BadRequest("Id mismatch");

            await _tradePointService.UpdateTradePointAsync(id, tradePointDto);
            return Ok();
        }*/

        [HttpDelete]
        public async Task<IActionResult> DeleteTradePoint([FromQuery] Guid guid)
        {
            await _tradePointService.DeleteTradePointAsync(guid);
            return Ok();
        }

        [HttpGet("{guid}", Name = "GetTradePointById")]
        public async Task<IActionResult> GetTradePointById([FromRoute] Guid guid)
        {
            var response = await _tradePointService.GetTradePointByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetAllTradePoints", Name = "GetAllTradePoints")]
        public async Task<IActionResult> GetAllTradePoint()
        {
            var response = await _tradePointService.GetAllTradePointsAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _tradePointService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }
    }
}

