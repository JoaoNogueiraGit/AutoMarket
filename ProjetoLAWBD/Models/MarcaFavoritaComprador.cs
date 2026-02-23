using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class MarcaFavoritaComprador {

        [Key]
        [Column("id_comprador")]
        public string IdComprador { get; set; }

        [Key]
        [Column("id_marca")]
        public int IdMarca { get; set; }

        [ForeignKey("IdComprador")]
        public virtual Comprador Comprador { get; set; }

        [ForeignKey("IdMarca")]
        public virtual Marca Marca { get; set; }
    }
}
