using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    [Table("WordMode")]
    public class WordMode
    {
        public int Id { get; set; }

        public int WordId { get; set; }

        public int ModoId { get; set; }

        public DateTime Fecha { get; set; }
    }
}
