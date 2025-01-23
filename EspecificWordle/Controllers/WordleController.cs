using EspecificWordle.Helpers;
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

        [Route("{modoDescripcion}")]
        public async Task<IActionResult> Index(string modoDescripcion)
        {
            var modoId = (await _IWordleService.GetModoByDescripcion(modoDescripcion)).Id;
            var word = CleanWord((await _IWordleService.GetModeWordDetailsAsync(modoId)).Palabra);

            var viewModel = new WordleViewModel
            {
                Length = word.Length,
                ModoId = modoId,
                ModoDescripcion = modoDescripcion,
                GameFinish = false,
                Juego = new Dictionary<string, List<Session>>()
            };

            #region Obtención de cookie

            var listSession = GetGameFromCookie(viewModel.ModoId);

            if (listSession.Count > 0)
                viewModel.Juego[viewModel.ModoId.ToString()] = listSession;

            viewModel.JuegoDictionaryJson = System.Text.Json.JsonSerializer.Serialize(viewModel.Juego);

            if (viewModel.Juego?.ContainsKey(modoId.ToString()) == true)
            {
                var juego = viewModel.Juego[modoId.ToString()];
                viewModel.Intentos = juego.Count;

                var ultimaPalabra = juego.Last().WordInsert.ToUpper();

                var resultado = ultimaPalabra == word.ToUpper() || (juego.Count == 5 && ultimaPalabra != word.ToUpper()) ?
                                (ultimaPalabra == word.ToUpper() ? 1 : 0) :
                                (int?)null;

                if (resultado.HasValue)
                {
                    viewModel.Resultado = resultado.Value;
                    viewModel.GameFinish = true;
                }
            }
            else
                viewModel.Intentos = 0;

            #endregion

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enter(WordleViewModel wordleViewModel)
        {
            var exist = await _IWordleService.WordCheckingAsync(wordleViewModel.PalabraIngresada, "es");
            var wordSecret = CleanWord((await _IWordleService.GetModeWordDetailsAsync(wordleViewModel.ModoId)).Palabra.ToUpper());

            if (wordleViewModel.JuegoDictionaryJson != null)
                wordleViewModel.Juego = JsonConvert.DeserializeObject<Dictionary<string, List<Session>>>(wordleViewModel.JuegoDictionaryJson);

            if (!wordleViewModel.Juego.ContainsKey(wordleViewModel.ModoId.ToString()))
                wordleViewModel.Juego.Add(wordleViewModel.ModoId.ToString(), new List<Session>());

            if (exist)
            {
                // Letras donde la posicion es correcta/incorrecta                    
                List<Letter> letters = new List<Letter>();

                for (int i = 0; i < wordSecret.Length; i++)
                {
                    if (wordSecret[i] == wordleViewModel.PalabraIngresada[i])
                        letters.Add(new Letter(){ Letra = wordleViewModel.PalabraIngresada[i].ToString(), Color = SystemConstants.ColorLetra.Verde, Acerted = true });
                    else
                    {
                        if (wordSecret.Contains(wordleViewModel.PalabraIngresada[i]))
                        {
                            bool isLetterRepeatedInYellowOrGray = letters.Any(letter =>
                                letter.Letra == wordleViewModel.PalabraIngresada[i].ToString() &&
                                letter.Color != SystemConstants.ColorLetra.Verde);

                            bool isLetterRepeatedInGreen = letters.Any(letter =>
                                letter.Letra == wordleViewModel.PalabraIngresada[i].ToString() &&
                                letter.Color == SystemConstants.ColorLetra.Verde);

                            bool isLetterRepeatedInWordSecret = wordSecret.GroupBy(character => character).Any(group => group.Count() > 1);

                            if ((!isLetterRepeatedInWordSecret && isLetterRepeatedInGreen) || isLetterRepeatedInYellowOrGray)
                            {
                                // 1er condición: Si la palabra oculta no repite alguna letra && Si el usuario ingresó una palabra que sí repite alguna letra y esta letra es correcta (Verde)
                                // 2da condición: Si el usuario ingresó una palabra que sí repite alguna letra y ésta letra ya se marcó (Amarilla o Gris)
                                letters.Add(new Letter(){ Letra = wordleViewModel.PalabraIngresada[i].ToString(), Color = SystemConstants.ColorLetra.Gris });
                            }
                            else    // Contiene la letra de la posición [i]
                                letters.Add(new Letter(){ Letra = wordleViewModel.PalabraIngresada[i].ToString(), Color = SystemConstants.ColorLetra.Amarillo });
                        }
                        else    // No contiene la letra
                            letters.Add(new Letter(){ Letra = wordleViewModel.PalabraIngresada[i].ToString(), Color = SystemConstants.ColorLetra.Gris });
                    }
                }

                wordleViewModel.Juego[wordleViewModel.ModoId.ToString()].Add(new Session() { 
                    Intento = wordleViewModel.Intentos,
                    WordInsert = wordleViewModel.PalabraIngresada,
                    Letters = letters
                });

                if (wordleViewModel.Intentos == 2)
                {
                    var juegoActual = wordleViewModel.Juego[wordleViewModel.ModoId.ToString()][wordleViewModel.Intentos];
                    juegoActual.Pista = PistaForUser(wordleViewModel.JuegoDictionaryJson, wordleViewModel.Length);
                    juegoActual.PistaDescripcion = juegoActual.Pista ? (await _IWordleService.GetModeWordDetailsAsync(wordleViewModel.ModoId)).Pista : string.Empty;
                }

                wordleViewModel.JuegoDictionaryJson = System.Text.Json.JsonSerializer.Serialize(wordleViewModel.Juego);

                wordleViewModel.Intentos++;

                // Si la palabra ingresada es correcta
                wordleViewModel.GameFinish = wordSecret.Equals(wordleViewModel.PalabraIngresada) || wordleViewModel.Intentos == 5;
                wordleViewModel.Resultado = wordSecret.Equals(wordleViewModel.PalabraIngresada) ? 1 : (wordleViewModel.Intentos == 5 ? 0 : wordleViewModel.Resultado);

                // Guarda en la cookie el json
                SaveGameInCookie(wordleViewModel.Juego, wordleViewModel.ModoId);

                return Json(wordleViewModel);
            }
            else
            {
                return Json(new { error = true, message = "La palabra ingresada no existe." });
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
            var letrasAcertadasPorLongitud = new Dictionary<int, int>
            {
                { 3, 1 }, { 4, 2 }, { 5, 3 }, { 6, 4 }, { 7, 5 }, { 8, 5 }, { 9, 6 }, { 10, 6 }
            };

            if (letrasAcertadasPorLongitud.ContainsKey(wordLength))
                return cantidadLetrasAcertadas <= letrasAcertadasPorLongitud[wordLength];

            return false;
        }

        [HttpGet]
        public async Task<IActionResult> Result(int result, int intento, int modoId, string modoDescripcion)
        {
            var aleatoriaDTO = await _IWordleService.GetModeWordDetailsAsync(modoId);

            var resultado = new ResultViewModel()
            {
                Intento = result == 1 ? ResultadoSegunIntento(intento) : "Que mal..",
                Palabra = aleatoriaDTO.Palabra,
                Definicion = aleatoriaDTO.Definicion,
                Sinonimos = aleatoriaDTO.Sinonimos,
                Antonimos = aleatoriaDTO.Antonimos,
                PalabraEn = aleatoriaDTO.PalabraEn,
                EjemploUso = aleatoriaDTO.EjemploUso,
                ModoDescripcion = modoDescripcion,
                Result = result == 1 ? 1 : 0
            };

            return PartialView("_Result", resultado);
        }

        [HttpGet]
        public IActionResult Instrucciones()
        {
            return PartialView("_Instrucciones");
        }

        private string CleanWord(string word)
        {
            var replacements = new Dictionary<char, char>
            {
                {'á', 'a'}, {'é', 'e'}, {'í', 'i'}, {'ó', 'o'}, {'ú', 'u'}, {'ä', 'a'}, {'ë', 'e'}, {'ï', 'i'}, {'ü', 'u'}, {'ö', 'o'},
                {'Á', 'A'}, {'É', 'E'}, {'Í', 'I'}, {'Ó', 'O'}, {'Ú', 'U'}, {'Ä', 'A'}, {'Ë', 'E'}, {'Ï', 'I'}, {'Ü', 'U'}, {'Ö', 'O'}
            };

            return new string(word.Select(c => replacements.ContainsKey(c) ? replacements[c] : c).ToArray());
        }

        private string ResultadoSegunIntento(int intento)
        {
            var resultados = new Dictionary<int, string>
            {
                { 1, SystemConstants.EstadoResult.Excelente }, { 2, SystemConstants.EstadoResult.Buenisimo },
                { 3, SystemConstants.EstadoResult.Aceptable }, { 4, SystemConstants.EstadoResult.Normal },
                { 5, SystemConstants.EstadoResult.Mejorable }
            };

            return resultados[intento];
        }

        #region Cookies

        public void SaveGameInCookie(Dictionary<string, List<Session>> juegoDictionary, int modoId)
        {
            try
            {
                var listSession = juegoDictionary[modoId.ToString()];
                var listSessionJson = JsonConvert.SerializeObject(listSession);

                #region Cálculo del tiempo de expiración de la cookie, hasta las 00:00

                var fechaActual = DateTimeOffset.Now;
                var proximaMedianoche = new DateTimeOffset(fechaActual.Year, fechaActual.Month, fechaActual.Day, 0, 0, 0, fechaActual.Offset).AddDays(1);
                var duracionHastaMedianoche = proximaMedianoche - fechaActual;

                #endregion

                Response.Cookies.Append($"GameByModo_{modoId}", listSessionJson, new CookieOptions
                {
                    Expires = fechaActual.Add(duracionHastaMedianoche),
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

        public List<Session> GetGameFromCookie(int modoId)
        {
            try
            {
                if (Request.Cookies.TryGetValue($"GameByModo_{modoId}", out var listSessionJson))
                {
                    var listSession = JsonConvert.DeserializeObject<List<Session>>(listSessionJson);
                    return listSession;
                }

                return new List<Session>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al recuperar la cookie: {ex.Message}");
            }
        }

        #endregion

    }
}
