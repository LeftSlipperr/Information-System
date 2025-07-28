using AutoMapper;
using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerService _workerService;
        private readonly IMapper _mapper;
        public WorkerController(IWorkerService workerService, IMapper mapper)
        {
            _workerService = workerService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task AddWorker([FromBody] WorkerDto workerDto)
        {
            if (workerDto == null)
            {
                BadRequest("balance cannot be null.");
            }

            await _workerService.AddWorkerAsync(workerDto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateWorker(Guid id, [FromBody] WorkerDto workerDto)
        {
            workerDto.WorkerId = id;
            await _workerService.UpdateWorkerAsync(id, workerDto);
            return Ok();
        }

        [HttpDelete]
        public async Task DeleteWorker([FromQuery] Guid guid)
        {
            await _workerService.DeleteWorkerAsync(guid);
        }

        [HttpGet("{guid}", Name = "GetWorkerById")]
        public async Task<IActionResult> GetWorkerById([FromRoute] Guid guid)
        {
            var response = await _workerService.GetWorkerByIdAsync(guid);

            return Ok(response);
        }

        [HttpGet("GetAllWorkers", Name = "GetAllWorkers")]
        public async Task<IActionResult> GetAllWorker()
        {
            var workers = await _workerService.GetAllWorkersAsync();
            var workersDto = _mapper.Map<List<WorkerDto>>(workers);
            return Ok(workersDto);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var result = await _workerService.SearchAsync(search, dateFrom, dateTo);
            return Ok(result);
        }
    }
}
