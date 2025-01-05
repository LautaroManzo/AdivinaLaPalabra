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
            var word = (await _IWordleService.GetAleatoriaAsync()).Palabra;

            var viewModel = new WordleViewModel
            {
                Length = word.Length,
                ModoId = 1
            };

            var cookie = GetGameFromCookie();
            viewModel.JuegoDictionaryJson = cookie == null ? string.Empty : cookie;
            var game = JsonConvert.DeserializeObject<Dictionary<string, List<Session>>>(cookie);

            if (game != null)
            {
                viewModel.Juego = game;
                viewModel.Intentos = game["1"].Count;

                // Buscar otra forma de hacer esto
                if (game["1"].Last().WordInsert.ToUpper() == word.ToUpper())
                    viewModel.Resultado = true;
                else if (game["1"].Count == 5 && game["1"].Last().WordInsert.ToUpper() != word.ToUpper())
                    viewModel.Resultado = false;
            }
            else
            {
                viewModel.Juego = new Dictionary<string, List<Session>>();
                viewModel.Intentos = 0;
            }

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
                // Letras donde la posicion es correcta/incorrecta                    
                List<Letter> letters = new List<Letter>();

                for (int i = 0; i < wordSecret.Length; i++)
                {
                    if (wordSecret[i] == wordleViewModel.PalabraIngresada[i])
                    {
                        letters.Add(new Letter() {
                            Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                            Color = "Verde",
                            Acerted = true
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
                                Color = "Amarillo",
                                Acerted = false
                            });
                        }
                        else
                        {
                            // No contiene la letra
                            letters.Add(new Letter()
                            {
                                Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                                Color = "Gris",
                                Acerted = false
                            });
                        }
                    }
                }

                wordleViewModel.Juego[wordleViewModel.ModoId.ToString()].Add(new Session() { 
                    Intento = wordleViewModel.Intentos,
                    WordInsert = wordleViewModel.PalabraIngresada,
                    Letters = letters
                });

                if (wordleViewModel.Intentos == 2)
                {
                    // [ModoId][0] cero ???????? Pensar en algo
                    wordleViewModel.Juego[wordleViewModel.ModoId.ToString()][wordleViewModel.Intentos].Pista = PistaForUser(wordleViewModel.JuegoDictionaryJson, wordleViewModel.Length);

                    if (wordleViewModel.Juego[wordleViewModel.ModoId.ToString()][wordleViewModel.Intentos].Pista)
                        wordleViewModel.Juego[wordleViewModel.ModoId.ToString()][wordleViewModel.Intentos].PistaDescripcion = (await _IWordleService.GetAleatoriaAsync()).Pista;
                }

                wordleViewModel.JuegoDictionaryJson = System.Text.Json.JsonSerializer.Serialize(wordleViewModel.Juego);

                if (wordleViewModel.Intentos != 4) wordleViewModel.Intentos++;
                else wordleViewModel.Resultado = false;
                
                // Si la palabra ingresada es correcta
                if (wordSecret.Equals(wordleViewModel.PalabraIngresada))
                    wordleViewModel.Resultado = true;

                // Acá guardaria en la cookie el json
                SaveGameInCookie(wordleViewModel.JuegoDictionaryJson);

                return Json(wordleViewModel);
            }
            else
            {
                return Json(new { error = true, message = "La palabra ingresada no existe." });
            }

        }

        public void SaveGameInCookie(string juegoDictionaryJson)
        {
            try
            {
                if (Request.Cookies.ContainsKey("WordCookie"))
                    Response.Cookies.Delete("WordCookie");

                Response.Cookies.Append("WordCookie", juegoDictionaryJson, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar cookie: {ex.Message}");
            }
        }

        public string GetGameFromCookie()
        {
            try
            {
                if (Request.Cookies.TryGetValue("WordCookie", out var juegoDictionaryJson))
                    return juegoDictionaryJson;

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al recuperar la cookie: {ex.Message}");
            }
        }

        public bool PistaForUser(string juegoDictionaryJson, int length)
        {
            Dictionary<string, List<Session>> juego;

            try
            {
                juego = JsonConvert.DeserializeObject<Dictionary<string, List<Session>>>(juegoDictionaryJson);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error en JSON.", ex);
            }

            // Letras acertadas únicas agrupadas por posición
            var letrasAcertadasUnicas = juego
                .SelectMany(juegoEntry => juegoEntry.Value)
                .SelectMany(sesion => sesion.Letters
                    .Select(
                        (letra, index) => new { 
                            Letra = letra, Index = index, sesion.Intento 
                        }
                    )
                )
                .Where(x => x.Letra.Acerted)
                .GroupBy(x => new { x.Index, x.Letra.Letra })
                .Select(g => g.First())
                .ToList();

            return EsNecesariaPista(letrasAcertadasUnicas.Count, length);
        }

        public bool EsNecesariaPista(int cantidadLetrasAcertadas, int wordLength)
        {
            bool debeMostrarPista = false;

            switch (wordLength)
            {
                case 5:
                    if (cantidadLetrasAcertadas <= 3)
                        debeMostrarPista = true;
                    break;
            }

            return debeMostrarPista;
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

        [HttpGet]
        public IActionResult Instrucciones()
        {
            return PartialView("_Instrucciones");
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
