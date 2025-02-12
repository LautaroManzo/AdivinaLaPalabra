using DataBase.Models;

namespace EspecificWordle.Models.Wordle
{
    public class ResultViewModel
    {
        public string Intento { get; set; }

        public string Palabra { get; set; }

        public string Definicion { get; set; }

        public List<Sinonimo> Sinonimos { get; set; }

        public List<Antonimo> Antonimos { get; set; }

        public string PalabraEn { get; set; }

        public string EjemploUso { get; set; }

        public string ModoDescripcion { get; set; }

        public int Result { get; set; }
    }
}
