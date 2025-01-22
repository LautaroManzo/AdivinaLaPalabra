using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataBase.Models
{
    [Table("Pista")]
    public class Pista
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("WordId")]
        public int WordId { get; set; }
        
        [Required]
        public string Descripcion { get; set; }
    }
}
