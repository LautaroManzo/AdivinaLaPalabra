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
            var palabra = "Palabra";
            
            return View(new WordleViewModel { Wordle = palabra.ToUpper() });
        }

        

    }
}
