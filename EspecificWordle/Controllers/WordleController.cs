using EspecificWordle.Interfaces;
using EspecificWordle.Models;
using EspecificWordle.Models.Wordle;
using Microsoft.AspNetCore.Mvc;

namespace EspecificWordle.Controllers
{
    public class WordleController : Controller
    {
        private readonly ILogger<WordleController> _logger;
        private readonly IWordleService _IWordleService;

        public WordleController(ILogger<WordleController> logger, IWordleService iWordleService)
        {
            _logger = logger;
            _IWordleService = iWordleService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new WordleViewModel
            {
                // Wordle = (await _IWordleService.GetAleatoriaAsync()).Palabra.ToUpper(),
                Tildes = false,
                Intentos = 0,
                Length = (await _IWordleService.GetAleatoriaAsync()).Palabra.Length
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enter(WordleViewModel wordleViewModel)
        {
            var exist = await _IWordleService.WordCheckingAsync(wordleViewModel.PalabraIngresada, "es");
            var word = (await _IWordleService.GetAleatoriaAsync()).Palabra.ToUpper();

            if (exist)
            {
                if (word.Equals(wordleViewModel.PalabraIngresada))
                {
                    wordleViewModel.Resultado = true;
                }
                else
                {
                    var palabraSecreta = word;
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
            var aleatoriaDTO = await _IWordleService.GetAleatoriaAsync(); // Diferenciar por modo

            var resultado = new ResultViewModel()
            {
                Intento = result ? ResultadoSegunIntento(intento) : "Que mal..",
                Palabra = aleatoriaDTO.Palabra,
                Definicion = aleatoriaDTO.Definicion,
                Sinonimos = aleatoriaDTO.Sinonimos,
                Antonimos = aleatoriaDTO.Antonimos,
                PalabraEn = aleatoriaDTO.PalabraEn,
                EjemploUso = aleatoriaDTO.EjemploUso,
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
