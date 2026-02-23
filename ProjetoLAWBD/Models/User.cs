using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models
{
    public class User : IdentityUser     
    {
        [PersonalData]
        public string? NomeCompleto { get; set; }

        [Column("created_at")]
        public DateTime DataRegisto { get; set; }
        public virtual Morada? Morada { get; set; }
        public virtual ICollection<Contacto>? Contactos { get; set; }
    }
}
