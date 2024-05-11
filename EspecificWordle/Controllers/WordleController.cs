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
                Intentos = 0
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enter(WordleViewModel wordleViewModel)
        {
            if (wordleViewModel.Wordle.Equals(wordleViewModel.PalabraIngresada))
            {
                // Si es correcta
            }
            else
            {
                var palabraSecreta = wordleViewModel.Wordle;
                var palabraIngresada = wordleViewModel.PalabraIngresada;

                bool[] letras = new bool[palabraSecreta.Length];    // Letras donde la posicion es correcta/incorrecta

                for (int i = 0; i < palabraSecreta.Length; i++)
                {
                    if (palabraSecreta[i] == palabraIngresada[i])
                    {
                        letras[i] = true;
                    }
                    else
                    {
                        letras[i] = false;
                    }
                }

                wordleViewModel.Posiciones = letras;
                wordleViewModel.Intentos++;
            }

            return Json(wordleViewModel);
        }        

    }
}
