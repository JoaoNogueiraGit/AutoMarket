using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class Bloqueio {

        [Key]
        [Column("id_bloqueio")]
        public int IdBloqueio { get; set; }

        [Required]
        [Column("id_utilizador_alvo")]
        public string IdUtilizadorAlvo { get; set; }

        [Required]
        [Column("id_administrador")]
        public string IdAdministrador { get; set; }

        [Required]
        [Column("data_bloqueio")]
        public DateTime DataBloqueio { get; set; }

        [Required]
        [Column("motivo")]
        public string Motivo { get; set; }

        [ForeignKey("IdUtilizadorAlvo")]
        public virtual User UtilizadorAlvo { get; set; }

        [ForeignKey("IdAdministrador")]
        public virtual Administrador Administrador { get; set; }
    }
}
