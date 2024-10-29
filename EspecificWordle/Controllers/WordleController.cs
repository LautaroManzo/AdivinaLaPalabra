using EspecificWordle.Interfaces;
using EspecificWordle.Models;
using EspecificWordle.Models.Wordle;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                Intentos = 0,
                Length = (await _IWordleService.GetAleatoriaAsync()).Palabra.Length,
                Juego = new Dictionary<string, List<Session>>(),
                ModoId = 1
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enter(WordleViewModel wordleViewModel)
        {
            var exist = await _IWordleService.WordCheckingAsync(wordleViewModel.PalabraIngresada, "es");
            var wordSecret = (await _IWordleService.GetAleatoriaAsync()).Palabra.ToUpper();

            if (wordleViewModel.JuegoDictionaryJson != null)
                wordleViewModel.Juego = JsonConvert.DeserializeObject<Dictionary<string, List<Session>>>(wordleViewModel.JuegoDictionaryJson);

            if (!wordleViewModel.Juego.ContainsKey(wordleViewModel.ModoId.ToString()))  // Mas modos?
                wordleViewModel.Juego.Add(wordleViewModel.ModoId.ToString(), new List<Session>());

            if (exist)
            {
                if (wordSecret.Equals(wordleViewModel.PalabraIngresada))
                {
                    wordleViewModel.Juego[wordleViewModel.ModoId.ToString()].Add(new Session() { Intento = 1, WordInsert = wordleViewModel.PalabraIngresada });
                    wordleViewModel.Resultado = true;
                }
                else
                {
                    // Letras donde la posicion es correcta/incorrecta                    
                    List<Letter> letters = new List<Letter>();

                    for (int i = 0; i < wordSecret.Length; i++)
                    {
                        if (wordSecret[i] == wordleViewModel.PalabraIngresada[i])
                        {
                            letters.Add(new Letter() {
                                Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                                Color = "Verde"
                            });
                        }
                        else
                        {
                            if (wordSecret.Contains(wordleViewModel.PalabraIngresada[i]))
                            {
                                // Contiene la letra de la posicion [i]
                                letters.Add(new Letter()
                                {
                                    Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                                    Color = "Amarillo"
                                });
                            }
                            else
                            {
                                // No contiene la letra
                                letters.Add(new Letter()
                                {
                                    Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                                    Color = "Gris"
                                });
                            }
                        }
                    }

                    wordleViewModel.Juego[wordleViewModel.ModoId.ToString()].Add(new Session() { 
                        Intento = wordleViewModel.Intentos,
                        WordInsert = wordleViewModel.PalabraIngresada,
                        Letters = letters
                    });

                    if (wordleViewModel.Intentos != 4) wordleViewModel.Intentos++;
                    else wordleViewModel.Resultado = false;
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
