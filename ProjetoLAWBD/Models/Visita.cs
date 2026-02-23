using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class Visita {

        [Key]
        [Column("id_visita")]
        public int IdVisita { get; set; }

        [Required]
        [Column("id_comprador")]
        public string IdComprador { get; set; }

        [Required]
        [Column("id_anuncio")]
        public int IdAnuncio { get; set; }

        [Required]
        [Column("estado_visita")]
        public string EstadoVisita { get; set; }

        [Required]
        [Column("data_pedido")]
        public DateTime DataPedido { get; set; }

        [Required]
        [Column("data_visita")]
        public DateTime DataVisita { get; set; }

        [ForeignKey("IdComprador")]
        public virtual Comprador Comprador { get; set; }

        [ForeignKey("IdAnuncio")]
        public virtual Anuncio Anuncio { get; set; }

    }
}
