using ExpenseApi.Common;
using ExpenseApi.DTOs;
using ExpenseApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(IExpenseService expenseService) : ApiControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _expenseService.GetAllAsync();

        return Success(result.Value!, "Expenses fetched successfully.");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _expenseService.GetByIdAsync(id);

        if (result.IsFailure)
            return Failure(result.Error!);

        return Success(result.Value!, "Expense fetched successfully.");
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        var result = await _expenseService.CreateAsync(dto);

        if (result.IsFailure)
            return Failure(result.Error!);

        return CreatedSuccess(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value,
            "Expense created successfully."
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto dto)
    {
        var result = await _expenseService.UpdateAsync(id, dto);

        if (result.IsFailure)
            return Failure(result.Error!);

        return Success(result.Value!, "Expense updated successfully.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _expenseService.DeleteAsync(id);

        if (result.IsFailure)
            return Failure(result.Error!);

        return Success(new { id }, "Expense deleted successfully.");
    }
}
