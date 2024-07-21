using EspecificWordle.Interfaces;
using EspecificWordle.Models;
using EspecificWordle.Models.ConfigApp;
using EspecificWordle.Models.Wordle;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace EspecificWordle.Controllers
{
    public class WordleController : Controller
    {
        private readonly ILogger<WordleController> _logger;
        private readonly IWordleService _IWordleService;
        private readonly ConfigApp _configApp;

        public WordleController(ILogger<WordleController> logger, ConfigApp configApp, IWordleService iWordleService)
        {
            _logger = logger;
            _configApp = configApp;
            _IWordleService = iWordleService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new WordleViewModel
            {
                Wordle = _configApp.RandomWord.ToUpper(),
                Tildes = false,
                Intentos = 0,
                Length = _configApp.RandomWord.Length
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enter(WordleViewModel wordleViewModel)
        {
            var exist = await _IWordleService.WordCheckingAsync(wordleViewModel.PalabraIngresada, "es");

            if (exist)
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
                            letras.Add(new
                            {
                                Letra = palabraIngresada[i].ToString(),
                                Color = "Verde"
                            });
                        }
                        else
                        {
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

                    if (wordleViewModel.Intentos != 4)
                        wordleViewModel.Intentos++;
                    else
                        wordleViewModel.Resultado = false;
                }

                return Json(wordleViewModel);
            }
            else
            {
                return Json(new { error = true, message = "La palabra ingresada no existe." });
            }

        }

        [HttpGet]
        public async Task<IActionResult> Result(bool result, int intento)
        {
            var resultado = new ResultViewModel()
            {
                Intento = result ? ResultadoSegunIntento(intento) : "Que mal..",
                Palabra = _configApp.RandomWord,
                Definicion = _configApp.RandomWordDef,
                DefinicionRae = _configApp.RandomWordDefRae,
                Sinonimos = string.Join(", ", _configApp.RandomWordSynonyms),
                Antonimos = string.Join(", ", _configApp.RandomWordAntonyms),
                PalabraEn = _configApp.RandomWordEn.ToLower(),
                EjemploUso = _configApp.RandomWordUseExamples,
                Result = result ? result : false
            };

            return PartialView("_Result", resultado);
        }

        private string ResultadoSegunIntento(int intento)
        {
            var result = string.Empty;

            switch (intento)
            {
                case 0:
                    result = EstadoResult.Excelente.ToString();
                    break;
                case 1:
                    result = EstadoResult.Buenisimo.ToString();
                    break;
                case 2:
                    result = EstadoResult.Aceptable.ToString();
                    break;
                case 3:
                    result = EstadoResult.Normal.ToString();
                    break;
                case 4:
                    result = EstadoResult.Mejorable.ToString();
                    break;
            }

            return result;
        }

        private enum EstadoResult
        {
            Excelente, Buenisimo, Aceptable, Normal, Mejorable
        }

    }
}
