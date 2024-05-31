namespace EspecificWordle.Models
{
    public class WordleViewModel
    {
        public string Wordle { get; set; }

        public bool Tildes { get; set; }

        public int Intentos { get; set; }

        public string PalabraIngresada { get; set; }

        public List<object> Letras { get; set; }

        public bool? Resultado { get; set; }
    }
}
