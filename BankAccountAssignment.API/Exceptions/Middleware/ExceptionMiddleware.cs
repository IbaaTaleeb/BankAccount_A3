using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace BankAccountAssignment.API.Exceptions.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionsAsync(context, ex);
            }

        }

        private  Task HandleExceptionsAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var errorDetails = new ErrorDetails()
            {
                ErrorType = "Failure",
                ErrorMessage = "An unexpected error occurred. Please try again later."
            };
            //check error type
            switch (ex)
            {
                case NotFoundException notFoundExceptions:
                    statusCode = HttpStatusCode.NotFound;
                    errorDetails.ErrorType = "Not found";
                    errorDetails.ErrorMessage = "An unexpected error occurred. Please try again later.";
                    break;

                case UnauthorizedAccessException unauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorDetails.ErrorType = "Unauthorized";
                    errorDetails.ErrorMessage = ex.Message;
                    break;

                case ArgumentException argumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorDetails.ErrorType = "Bad Request";
                    errorDetails.ErrorMessage = argumentException.Message;
                    break;

                default:
                    _logger.LogError($"Unhandled exception: {ex.Message}"); // Log other unexpected errors
                    break;
            }
            string response = JsonConvert.SerializeObject(errorDetails);
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(response);
        }
    }
}
