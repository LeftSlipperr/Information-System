using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }
    
    [HttpPost(Name = "AddExpense")]
    public async Task AddExpense([FromBody] ExpenseDto expenseDto)
    {
        if (expenseDto == null)
        {
            BadRequest("balance cannot be null.");
        }

        await _expenseService.AddExpenseAsync(expenseDto);

    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] ExpenseDto expenseDto)
    {
        expenseDto.ExpenseId = id;
        await _expenseService.UpdateExpenseAsync(id, expenseDto);
        return Ok();
    }

    [HttpDelete(Name = "DeleteExpense")]
    public async Task DeleteExpense([FromQuery] Guid guid)
    {
        await _expenseService.DeleteExpenseAsync(guid);
    }

    [HttpGet("{guid}", Name = "GetExpenseById")]
    public async Task<IActionResult> GetExpenseById([FromRoute] Guid guid)
    {
        var response = await _expenseService.GetExpenseByIdAsync(guid);

        return Ok(response);
    }

    [HttpGet("GetExpenseByPeriod", Name = "GetExpenseByPeriod")]
    public async Task<IActionResult> GetExpenseByPeriod([FromQuery] DateTime period)
    {
        var response = await _expenseService.GetExpenseByPeriodAsync(period);

        return Ok(response);
    }

    [HttpGet("GetAllExpenses", Name = "GetAllExpenses")]
    public async Task<IActionResult> GetAllExpenses()
    {
        var response = await _expenseService.GetAllExpensesAsync();

        return Ok(response);
    }

    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var result = await _expenseService.SearchAsync(search, dateFrom, dateTo);
        return Ok(result);
    }
}