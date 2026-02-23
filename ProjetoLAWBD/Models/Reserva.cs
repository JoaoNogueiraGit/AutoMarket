using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class Reserva {

        [Key]
        [Column("id_reserva")]
        public int IdReserva { get; set; }

        [Required]
        [Column("id_comprador")]
        public string IdComprador { get; set; }

        [Required]
        [Column("id_anuncio")]
        public int IdAnuncio { get; set; }

        [Required]
        [Column("estado_reserva")]
        public string EstadoReserva { get; set; }

        [Required]
        [Column("data_pedido")]
        public DateTime DataPedido { get; set; }

        [Required]
        [Column("prazo_expiracao")]
        public DateTime PrazoExpiracao { get; set; } 

        [ForeignKey("IdComprador")]
        public virtual Comprador Comprador { get; set; }

        [ForeignKey("IdAnuncio")]
        public virtual Anuncio Anuncio { get; set; }
    }
}
