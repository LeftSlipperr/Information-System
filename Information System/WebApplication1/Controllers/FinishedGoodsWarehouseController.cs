using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinishedGoodsWarehouseController : ControllerBase
    {
        private readonly IFinishedGoodsWarehouseService _finishedGoodsWarehouseService;
        public FinishedGoodsWarehouseController(IFinishedGoodsWarehouseService finishedGoodsWarehouseService)
        {
            _finishedGoodsWarehouseService = finishedGoodsWarehouseService;
        }

        [HttpPost]
        public async Task AddFinishedGoodsWarehouse([FromBody] FinishedGoodsWarehouseDto finishedGoodsWarehouseDto)
        {
            if (finishedGoodsWarehouseDto == null)
            {
                BadRequest("balance cannot be null.");
            }

            await _finishedGoodsWarehouseService.AddFinishedGoodsWarehouseAsync(finishedGoodsWarehouseDto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateFinishedGoodsWarehouse(Guid id, [FromBody] FinishedGoodsWarehouseDto finishedGoodsWarehouseDto)
        {
            finishedGoodsWarehouseDto.ProductId = id;
            await _finishedGoodsWarehouseService.UpdateFinishedGoodsWarehouseAsync(id, finishedGoodsWarehouseDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteFinishedGoodsWarehouse([FromQuery] Guid guid)
        {
            await _finishedGoodsWarehouseService.DeleteFinishedGoodsWarehouseAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetFinishedGoodsWarehouseById")]
        public async Task<IActionResult> GetFinishedGoodsWarehouseById([FromRoute] Guid guid)
        {
            var response = await _finishedGoodsWarehouseService.GetFinishedGoodsWarehouseByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetFinishedGoodsWarehouseByPeriod", Name = "GetFinishedGoodsWarehouseByPeriod")]
        public async Task<IActionResult> GetFinishedGoodsWarehouseByPeriod([FromQuery] DateTime period)
        {
            var response = await _finishedGoodsWarehouseService.GetFinishedGoodsWarehouseByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllFinishedGoodsWarehouses", Name = "GetAllFinishedGoodsWarehouses")]
        public async Task<IActionResult> GetAllFinishedGoodsWarehouse()
        {
            var response = await _finishedGoodsWarehouseService.GetAllFinishedGoodsWarehousesAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _finishedGoodsWarehouseService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }

    }
}
