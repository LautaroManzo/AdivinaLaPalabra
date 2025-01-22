using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataBase.Models
{
    [Table("Modo")]
    public class Modo
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string Descripcion { get; set; }
    }
}
