using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataBase.Models
{
    [Table("PalabraEn")]
    public class PalabraEn
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
