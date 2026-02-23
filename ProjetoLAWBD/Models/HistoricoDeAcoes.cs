using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class HistoricoDeAcoes {

        [Key]
        [Column("id_acao")]
        public int IdAcao { get; set; }

        [Column("id_administrador_autor")]
        public string? IdAdministradorAutor { get; set; } 

        [Required]
        [Column("data_acao")]
        public DateTime DataAcao { get; set; }

        [Required]
        [Column("tipo_acao")]
        public string TipoAcao { get; set; }

        [Required]
        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("entidade_alvo")]
        public string? EntidadeAlvo { get; set; } 

        [Column("id_entidade_alvo")]
        public string? IdEntidadeAlvo { get; set; } 

        [ForeignKey("IdAdministradorAutor")]
        public virtual Administrador? AdministradorAutor { get; set; }
    }
}
