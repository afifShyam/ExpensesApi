using ExpenseApi.Application.DTOs.Commitment;
using ExpenseApi.Application.Services.Commitments;
using ExpenseApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class CommitmentController(ICommitmentService commitmentService) : ApiControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(ApiSuccessResponse<IEnumerable<CommitmentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await commitmentService.GetAllAsync();

        return result.When(
            success: value => Success(value, "Commitments fetched successfully."),
            failure: Failure
        );
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiSuccessResponse<CommitmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await commitmentService.GetByIdAsync(id);

        return result.When(
            success: value => Success(value, "Commitment fetched successfully."),
            failure: Failure
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiSuccessResponse<CommitmentResponseDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateCommitmentDto dto)
    {
        var result = await commitmentService.CreateAsync(dto);

        return result.When(
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
    [ProducesResponseType(typeof(ApiSuccessResponse<CommitmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCommitmentDto dto)
    {
        var result = await commitmentService.UpdateAsync(id, dto);

        return result.When(
            success: value => Success(value, "Commitment updated successfully."),
            failure: Failure
        );
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiSuccessResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await commitmentService.DeleteAsync(id);

        return result.When(
            success: value => Success(value, "Commitment deleted successfully."),
            failure: Failure
        );
    }

    [HttpDelete("all")]
    [ProducesResponseType(typeof(ApiSuccessResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAll()
    {
        var result = await commitmentService.DeleteAllAsync();

        return result.When(
            success: value => Success(value, "All commitments deleted successfully."),
            failure: Failure
        );
    }
}