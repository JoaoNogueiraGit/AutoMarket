using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ProjetoLAWBD.Models {
    public class ConfNotificacoes {

        [Key]
        [Column("id_utilizador")]
        public string IdUtilizador { get; set; }

        [Required(ErrorMessage = "Obrigatório escolher uma opção")]
        [Column("anuncios_marcas_favoritas")]
        public bool AnunciosMarcasFavoritas { get; set; }

        [Required(ErrorMessage = "Obrigatório escolher uma opção")]
        [Column("alteracoes_reservas_visitas")]
        public bool AlteracoesReservasVisitas { get; set; }


        [ForeignKey("IdUtilizador")]
        public virtual User User { get; set; }
    }
}
