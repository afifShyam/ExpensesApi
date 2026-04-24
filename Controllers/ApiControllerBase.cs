using ExpenseApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Success<T>(T data, string message = "Success")
    {
        return Ok(new ApiSuccessResponse<T>
        {
            Message = message,
            Data = data
        });
    }

    protected IActionResult CreatedSuccess<T>(
        string actionName,
        object routeValues,
        T data,
        string message = "Created successfully")
    {
        return CreatedAtAction(
            actionName,
            routeValues,
            new ApiSuccessResponse<T>
            {
                Message = message,
                Data = data
            }
        );
    }

    protected IActionResult Failure(Error error)
    {
        var response = new ApiErrorResponse
        {
            Message = error.Message,
            Error = new ApiError
            {
                Code = error.Code,
                TraceId = HttpContext.TraceIdentifier,
                Details = error.Details
            }
        };

        return error.Code switch
        {
            "Expense.NotFound" => NotFound(response),
            "VALIDATION_ERROR" => BadRequest(response),
            "AUTH_EXPIRED" => Unauthorized(response),
            _ => BadRequest(response)
        };
    }
}
