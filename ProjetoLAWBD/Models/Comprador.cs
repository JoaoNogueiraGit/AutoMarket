using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models
{
    public class Comprador
    {
        [Key]
        [Column("id_utilizador")]
        public string IdUtilizador {  get; set; }

        [ForeignKey("IdUtilizador")]
        public virtual User user { get; set; }

    }
}
