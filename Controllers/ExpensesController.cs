using ExpenseApi.Application.DTOs.Expenses;
using ExpenseApi.Application.Services.Expenses;
using ExpenseApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ExpensesController(IExpenseService expenseService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiSuccessResponse<IEnumerable<ExpenseResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await expenseService.GetAllAsync();

        return result.When(
            success: value => Success(value, "Expenses fetched successfully."),
            failure: Failure);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiSuccessResponse<ExpenseResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await expenseService.GetByIdAsync(id);

        return result.When(
            success: value => Success(value, "Expense fetched successfully."),
            failure: Failure);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiSuccessResponse<ExpenseResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
    {
        var result = await expenseService.CreateAsync(dto);

        return result.When(
            success: value => CreatedSuccess(
                nameof(GetById),
                new { id = value!.Id },
                value,
                "Expense created successfully."),
            failure: Failure);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiSuccessResponse<ExpenseResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateExpenseDto dto)
    {
        var result = await expenseService.UpdateAsync(id, dto);

        return result.When(
            success: value => Success(value, "Expense updated successfully."),
            failure: Failure);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiSuccessResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await expenseService.DeleteAsync(id);

        return result.When(
            success: value => Success(value, "Expense deleted successfully."),
            failure: Failure);
    }
}