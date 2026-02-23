using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class Morada {

        [Key]
        [Column("id_utilizador")]
        public string IdUtilizador { get; set; }

        [Required]
        [Column("rua_e_numero")]
        public string RuaENumero { get; set; }

        [Required]
        [Column("codigo_postal")]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "O código postal deve ter o formato 0000-000.")]
        public string CodigoPostal { get; set; }

        [Required]
        [Column("distrito")]
        public string Distrito { get; set; }

        [Required]
        [Column("cidade")]
        public string Cidade { get; set; }

        [ForeignKey("IdUtilizador")]
        public virtual User user { get; set; }

        
    }
}
