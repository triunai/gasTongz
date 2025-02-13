using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.Logging;

namespace _4_GasTongz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BaseController> _logger;

        public BaseController(IMediator mediator, ILogger<BaseController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Success response method  
        protected IActionResult SuccessResponse<T>(T data, string message = "Success")
           => Ok(new ApiResponse<T>(true, message, data));

        // 201 Created response
        protected IActionResult CreatedResponse<T>(T data, string location = null, string message = "Created")
        {
            if (!string.IsNullOrEmpty(location))
            {
                return Created(location, new ApiResponse<T>(true, message, data));
            }
            else
            {
                // When location is not provided, simply return CreatedAtAction with null route values.
                return CreatedAtAction(null, new ApiResponse<T>(true, message, data));
            }
        }

        // 204 No Content
        protected IActionResult NoContentResponse()
            => NoContent();

        // 400 Bad Request response with details
        protected IActionResult BadRequestResponse(string errorMessage)
        {
            _logger.LogError(errorMessage);
            return BadRequest(new ApiResponse<object>(false, errorMessage));
        }

        // 404 Not Found response
        protected IActionResult NotFoundResponse(string errorMessage = "Resource not found")
        {
            _logger.LogWarning(errorMessage);
            return NotFound(new ApiResponse<object>(false, errorMessage));
        }

        // Generic error response
        protected IActionResult ErrorResponse(string errorMessage, int statusCode = 500)
        {
            _logger.LogError(errorMessage);
            return StatusCode(statusCode, new ApiResponse<object>(false, errorMessage));
        }

        // MediatR helper method  
        protected async Task<IActionResult> SendRequest<TResponse>(IRequest<TResponse> request, string successMessage = "Success")
        {
            try
            {
                var response = await _mediator.Send(request);
                if (response == null)
                {
                    return NotFoundResponse("Resource not found");
                }
                if (response is int id && id == 0)
                {
                    return BadRequestResponse("Operation failed.");
                }

                return SuccessResponse(response, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request.");
                return ErrorResponse("An error occurred while processing your request.", 500);
            }
        }
    }

    // Standard API response model  
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(bool success, string message, T data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
