using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawMaterialWarehouseController : ControllerBase
    {
        private readonly IRawMaterialWarehouseService _rawMaterialWarehouseService;
        public RawMaterialWarehouseController(IRawMaterialWarehouseService rawMaterialWarehouseService)
        {
            _rawMaterialWarehouseService = rawMaterialWarehouseService;
        }

        [HttpPost]
        public async Task AddRawMaterialWarehouse([FromBody] RawMaterialWarehouseDto rawMaterialWarehouseDto)
        {
            if (rawMaterialWarehouseDto == null)
            {
                BadRequest("balance cannot be null.");
            }

            await _rawMaterialWarehouseService.AddRawMaterialAsync(rawMaterialWarehouseDto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateRawMaterialWarehouse(Guid id, [FromBody] RawMaterialWarehouseDto rawMaterialWarehouseDto)
        {
            rawMaterialWarehouseDto.MaterialId = id;
            await _rawMaterialWarehouseService.UpdateRawMaterialAsync(id, rawMaterialWarehouseDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteRawMaterialWarehouse([FromQuery] Guid guid)
        {
            await _rawMaterialWarehouseService.DeleteRawMaterialAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetRawMaterialWarehouseById")]
        public async Task<IActionResult> GetRawMaterialWarehouseById([FromRoute] Guid guid)
        {
            var response = await _rawMaterialWarehouseService.GetRawMaterialWarehouseByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetRawMaterialWarehouseByPeriod", Name = "GetRawMaterialWarehouseByPeriod")]
        public async Task<IActionResult> GetRawMaterialWarehouseByPeriod([FromQuery] DateTime period)
        {
            var response = await _rawMaterialWarehouseService.GetRawMaterialWarehousesByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllRawMaterialWarehouses", Name = "GetAllRawMaterialWarehouses")]
        public async Task<IActionResult> GetAllRawMaterialWarehouse()
        {
            var response = await _rawMaterialWarehouseService.GetAllRawMaterialsAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _rawMaterialWarehouseService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }
    }
}
