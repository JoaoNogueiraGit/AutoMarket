using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class Combustivel {

        [Key]
        [Column("id_combustivel")]
        public int IdCombustivel { get; set; }

        [Required]
        [Column("nome")]
        
        public string Nome { get; set; }
    }
}
