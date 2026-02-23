using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class Modelo {

        [Key]
        [Column("id_modelo")]
        public int IdModelo { get; set; }

        [Required]
        [Column("id_marca")]
        public int IdMarca { get; set; }

        [Required]
        [Column("nome")]
        public string Nome { get; set; }

        [ForeignKey("IdMarca")]
        public virtual Marca Marca { get; set; }
    }
}
