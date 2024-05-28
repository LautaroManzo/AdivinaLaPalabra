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
                wordleViewModel.Resultado = true;
            }
            else
            {
                var palabraSecreta = wordleViewModel.Wordle;
                var palabraIngresada = wordleViewModel.PalabraIngresada;

                // Letras donde la posicion es correcta/incorrecta
                List<object> letras = new List<object>();

                for (int i = 0; i < palabraSecreta.Length; i++)
                {
                    if (palabraSecreta[i] == palabraIngresada[i])
                    {
                        letras.Add(new { 
                            Letra = palabraIngresada[i].ToString(),
                            Color = "Verde"
                        });
                    }
                    else
                    {
                        // P A L A B R A
                        // P A R T I D O

                        if (palabraSecreta.Contains(palabraIngresada[i]))
                        {
                            // Contiene la letra de la posicion [i]
                            letras.Add(new
                            {
                                Letra = palabraIngresada[i].ToString(),
                                Color = "Amarillo"
                            });
                        }
                        else
                        {
                            // No contiene la letra
                            letras.Add(new
                            {
                                Letra = palabraIngresada[i].ToString(),
                                Color = "Gris"
                            });
                        }

                    }
                }

                wordleViewModel.Letras = letras;
                wordleViewModel.Intentos++;
            }

            return Json(wordleViewModel);
        }

        [HttpPost]
        public IActionResult Result(string palabra)
        {
            var resultado = new
            {
                Palabra = palabra,
                Significado = "Test test palabra partida partido probando, texto largo. Esto esta andando bien.",
                Info = "Esto es solo de prueba.", 
                Result = true
            };
            
            return Json(resultado);
        }

    }
}
