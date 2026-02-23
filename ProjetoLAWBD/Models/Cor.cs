using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class Cor {

        [Key]
        [Column("id_cor")]
        public int IdCor { get; set; }

        [Required]
        [Column("nome")]
        // UNIQUE
        public string Nome { get; set; }
    }
}
