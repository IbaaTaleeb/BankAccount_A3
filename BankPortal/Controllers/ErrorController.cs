using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankPortal.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Log the error details
            _logger.LogError("An unexpected error occurred.");

            // Display a generic error message
            ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
            return View();
        }
    }
}
