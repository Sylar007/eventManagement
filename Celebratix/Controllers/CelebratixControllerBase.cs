using Celebratix.Models;
using F = CSharpFunctionalExtensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers
{
    public abstract class CelebratixControllerBase : ControllerBase
    {
        protected ActionResult<ResultDto<T, E>> ToResponse<T, E>(F.Result<T, E> result)
        {
            var operationResult = new ResultDto<T, E>(result);
            return result.IsSuccess
                ? new ObjectResult(operationResult) { StatusCode = StatusCodes.Status200OK }
                : new ObjectResult(operationResult) { StatusCode = StatusCodes.Status400BadRequest };
        }
        protected IActionResult GetOkResult<T>(Result result)
        {
            var operationResult = new OperationResult<T>(result);

            return result.IsSuccess
                ? Ok(operationResult)
                : BadRequest(operationResult);
        }

        protected IActionResult GetOkResult<T>(Result<T> result)
        {
            var operationResult = new OperationResult<T>(result);

            return result.IsSuccess
                ? Ok(operationResult)
                : BadRequest(operationResult);
        }

        protected IActionResult GetCreatedResult<T>(Result<T> result)
        {
            var operationResult = new OperationResult<T>(result);

            return result.IsSuccess
                ? new ObjectResult(operationResult) { StatusCode = StatusCodes.Status201Created }
                : BadRequest(operationResult);
        }

        protected IActionResult GetCreatedResult<T>(Result result)
        {
            var operationResult = new OperationResult<T>(result);

            return result.IsSuccess
                ? new ObjectResult(operationResult) { StatusCode = StatusCodes.Status201Created }
                : BadRequest(operationResult);
        }

        protected IActionResult GetNoContentResult<T>(Result<T> result)
        {
            var operationResult = new OperationResult<T>(result);

            return result.IsSuccess
                ? NoContent()
                : BadRequest(operationResult);
        }

        protected IActionResult GetNoContentResult<T>(Result result)
        {
            var operationResult = new OperationResult<T>(result);

            return result.IsSuccess
                ? NoContent()
                : BadRequest(operationResult);
        }
    }
}
