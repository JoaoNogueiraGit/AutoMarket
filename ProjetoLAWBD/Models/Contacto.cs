using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class Contacto {

        [Key]
        [Column("id_contacto")]
        public int IdContacto { get; set; }

        [Required]
        [Column("id_utilizador")]
        public string IdUtilizador { get; set; }

        [Required]
        [Column("tipo_contacto")]
        public string TipoContacto { get; set; }

        [Required]
        [Column("contacto")]
        public string ValorContacto { get; set; }

        [ForeignKey("IdUtilizador")]
        public virtual User User { get; set; }
    }
}
