using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class Categoria {

        [Key]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Required]
        [Column("nome")]
        // UNIQUE
        public string Nome { get; set; }
    }
}
