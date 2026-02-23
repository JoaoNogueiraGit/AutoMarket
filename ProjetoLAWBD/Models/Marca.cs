using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class Marca {

        [Key]
        [Column("id_marca")]
        public int IdMarca { get; set; }

        [Required]
        [Column("nome")]
        // UNIQUE
        public string Nome { get; set; }
    }
}
