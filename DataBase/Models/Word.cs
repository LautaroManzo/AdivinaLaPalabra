using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace DataBase.Models
{
    [Table("Word")]
    public class Word
    {
        [Key]
        public int Id { get; set; }

        public int? ModoId { get; set; }

        [Required, DefaultValue(false)]
        public bool Usada { get; set; }

        [Required, StringLength(16)]
        public string Descripcion { get; set; }

        public virtual Uso Uso { get; set; }
        public virtual Pista Pista { get; set; }
        public virtual PalabraEn PalabraEn { get; set; }
        public virtual Definicion Definicion { get; set; }
        public virtual ICollection<Sinonimo> Sinonimo { get; set; }
        public virtual ICollection<Antonimo> Antonimo { get; set; }
    }
}
