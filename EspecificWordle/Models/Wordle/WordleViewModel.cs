namespace EspecificWordle.Models
{
    public class WordleViewModel
    {
        public int Intentos { get; set; }

        public int Length { get; set; }

        public int ModoId { get; set; }

        public string ModoDescripcion { get; set; }

        public string PalabraIngresada { get; set; }

        public int Resultado { get; set; }

        public bool GameFinish { get; set; }

        public Dictionary<string, List<Session>> Juego { get; set; }

        public string JuegoDictionaryJson { get; set; }
    }

    public class Session
    {
        public int Intento { get; set; }

        public string WordInsert { get; set; }

        public bool Pista { get; set; }

        public string PistaDescripcion { get; set; }

        public List<Letter> Letters { get; set; }
    }

    public class Letter
    {
        public string Letra { get; set; }

        public string Color { get; set; }

        public bool Acerted { get; set; }
    }

}
