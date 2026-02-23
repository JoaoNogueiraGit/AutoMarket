using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class Caixa {

        [Key]
        [Column("id_caixa")]
        public int IdCaixa { get; set; }

        [Required]
        [Column("nome")]
        // UNIQUE
        public string Nome { get; set; }
    }
}
