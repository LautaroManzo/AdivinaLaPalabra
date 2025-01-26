using EspecificWordle.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EspecificWordle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Wordle", new { modoDescripcion = "Aleatoria" });  // El modo por default es 'Aleatoria'
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ExceptionMessage = exceptionDetails?.Error.Message,
                StackTrace = exceptionDetails?.Error.StackTrace,
                StackTraceByEmail = exceptionDetails?.Error.StackTrace?.Split('\n')[0],
                Path = exceptionDetails?.Path
            };

            return View("~/Views/Shared/Error.cshtml", errorViewModel);
        }

    }
}
