using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        public IncidentController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        [HttpPost]
        public async Task AddIncident([FromBody] IncidentDto incidentDto)
        {
            if (incidentDto == null)
            {
                BadRequest("balance cannot be null.");
            }

            await _incidentService.AddIncidentAsync(incidentDto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateIncident(Guid id, [FromBody] IncidentDto incidentDto)
        {
            incidentDto.IncidentId = id;
            await _incidentService.UpdateIncidentAsync(id, incidentDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteFinishedGoodsWarehouse([FromQuery] Guid guid)
        {
            await _incidentService.DeleteIncidentAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetIncidentById")]
        public async Task<IActionResult> GetIncidentById([FromRoute] Guid guid)
        {
            var response = await _incidentService.GetIncidentByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetIncidentByPeriod", Name = "GetIncidentByPeriod")]
        public async Task<IActionResult> GetIncidentByPeriod([FromQuery] DateTime period)
        {
            var response = await _incidentService.GetIncidentByPeriodAsync(period);

            return Ok(response);
        }

        [HttpGet("GetAllIncidents", Name = "GetAllIncidents")]
        public async Task<IActionResult> GetAllIncident()
        {
            var response = await _incidentService.GetAllIncidentsAsync();

            return Ok(response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _incidentService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }
    }
}
