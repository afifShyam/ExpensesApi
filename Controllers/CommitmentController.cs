using ExpenseApi.Application.DTOs.Commitment;
using ExpenseApi.Application.Services.Commitments;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommitmentController(ICommitmentService commitmentService) : ApiControllerBase
{
    private readonly ICommitmentService _commitmentService = commitmentService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _commitmentService.GetAllAsync();

        return result.When<IActionResult>(
            success: value => Success(value, "Commitments fetched successfully."),
            failure: Failure
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _commitmentService.GetByIdAsync(id);

        return result.When<IActionResult>(
            success: value => Success(value, "Commitment fetched successfully."),
            failure: Failure
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommitmentDto dto)
    {
        var result = await _commitmentService.CreateAsync(dto);

        return result.When<IActionResult>(
            success: value => CreatedSuccess(
                nameof(GetById),
                new { id = value!.Id },
                value,
                "Commitment created successfully."
            ),
            failure: Failure
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCommitmentDto dto)
    {
        var result = await _commitmentService.UpdateAsync(id, dto);

        return result.When<IActionResult>(
            success: value => Success(value, "Commitment updated successfully."),
            failure: Failure
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _commitmentService.DeleteAsync(id);

        return result.When<IActionResult>(
            success: value => Success(value, "Commitment deleted successfully."),
            failure: Failure
        );
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAll()
    {
        var result = await _commitmentService.DeleteAllAsync();

        return result.When<IActionResult>(
            success: value => Success(value, "All commitments deleted successfully."),
            failure: Failure
        );
    }
}