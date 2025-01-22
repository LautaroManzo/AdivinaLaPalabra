using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    [Table("Antonimo")]
    public class Antonimo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("WordId")]
        public int WordId { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Descripcion { get; set; }
    }
}
