using FluentValidation; // For FluentValidation.ValidationException
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _4_GasTongz.API.Middleware
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Log the exception details
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            // Create a ProblemDetails instance based on the exception type
            var problemDetails = CreateProblemDetails(httpContext, exception);

            // Set the response status code and content type
            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/problem+json";

            // Write the ProblemDetails as JSON to the response
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

            return true;
        }

        private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
        {
            // Retrieve the trace identifier from the HttpContext for correlation
            var traceId = context.TraceIdentifier;
            ProblemDetails problemDetails;

            // Map exception types to specific ProblemDetails responses
            switch (exception)
            {
                // FluentValidation exception: return a 400 Bad Request with validation errors
                case ValidationException validationException:
                    problemDetails = new ValidationProblemDetails(validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "One or more validation errors occurred.",
                        Detail = exception.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // Custom NotFoundException: return a 404 Not Found
                // (Assuming you have a custom NotFoundException in your domain)
                case NotFoundException notFoundException:
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Resource not found",
                        Detail = notFoundException.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // UnauthorizedAccessException: return a 401 Unauthorized
                case UnauthorizedAccessException unauthorizedAccess:
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = unauthorizedAccess.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // Default: return a 500 Internal Server Error for unexpected exceptions
                default:
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occurred.",
                        Detail = exception.Message,
                        Instance = context.Request.Path
                    };
                    break;
            }

            // Enrich the ProblemDetails with additional context (e.g., traceId)
            problemDetails.Extensions.Add("traceId", traceId);
            // You can add more custom properties here as needed
            // For example: problemDetails.Extensions.Add("errorCode", "E12345");

            return problemDetails;
        }
    }

    internal class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
