using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models
{
    public class Administrador
    {
        [Key]
        [Column("id_utilizador")]
        public string IdUtilizador { get; set; }

        [Column("id_administrador_criador")]
        public string? IdAdministradorCriador { get; set; }

        [ForeignKey("IdUtilizador")]
        public virtual User user { get; set; }

        [ForeignKey("IdAdministradorCriador")]
        public virtual Administrador? AdminCriador { get; set; }
    }
}
