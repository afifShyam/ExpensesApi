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


        return result.When<IActionResult>(
            success: value => Success(value, "Expenses fetched successfully."),
            failure: error => Failure(error)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _expenseService.GetByIdAsync(id);

        return result.When<IActionResult>(
            success: value => Success(value, "Expense fetched successfully."),
            failure: Failure
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        var result = await _expenseService.CreateAsync(dto);

        return result.When<IActionResult>(
            success: value => CreatedSuccess(
            nameof(GetById),
            new { id = value!.Id },
            value,
            "Expense created successfully."
        ),
            failure: Failure
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto dto)
    {
        var result = await _expenseService.UpdateAsync(id, dto);

        return result.When<IActionResult>(
            success: value => Success(value, "Expense updated successfully."),
            failure: Failure
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _expenseService.DeleteAsync(id);

        return result.When<IActionResult>(
            success: value => Success(value, "Expense deleted successfully."),
            failure: Failure
        );
    }
}
