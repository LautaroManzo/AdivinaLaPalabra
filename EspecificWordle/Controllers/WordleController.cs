using EspecificWordle.Models;
using Microsoft.AspNetCore.Mvc;

namespace EspecificWordle.Controllers
{
    public class WordleController : Controller
    {
        private readonly ILogger<WordleController> _logger;

        public WordleController(ILogger<WordleController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewModel = new WordleViewModel {
                Wordle = "Palabra".ToUpper(),
                Tildes = false,
                Intentos = 1
            };

            return View(viewModel);
        }

        

    }
}
