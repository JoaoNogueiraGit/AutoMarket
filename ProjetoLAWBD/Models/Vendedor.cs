using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models
{
    public class Vendedor
    {
        [Key]
        [Column("id_utilizador")]
        public string IdUtilizador { get; set; }

        [Required(ErrorMessage = "O tipo de utilizador é obrigatório.")]
        [Column("tipo_vendedor")]
        public string TipoVendedor { get; set; }

        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O NIF deve ter exatamente 9 números.")]
        [Column("nif")]
        public string NIF { get; set; }

        [Column("id_administrador_validador")]
        public string? IdAdministradorValidador { get; set; }

        [Required]
        [Column("estado_validacao")]
        public string EstadoValidacao { get; set; } = "Pendente";

        [Column("data_validacao")]
        public DateTime? DataValidacao { get; set; }

        [Column("nome_faturacao")]
        public string? NomeFaturacao { get; set; }

        [Column("morada_faturacao_rua")]
        public string? RuaFaturacao { get; set; }

        [Column("morada_faturacao_codigo_postal")]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "O código postal deve ter o formato 0000-000.")]
        public string? CPFaturacao { get; set; }

        [Column("morada_faturacao_cidade")]
        public string? CidadeFaturacao { get; set; }

        [Column("iban")]
        [RegularExpression(@"^PT50\d{21}$", ErrorMessage = "O IBAN (PT) deve estar no formato PT50 seguido de 21 números.")]
        public string? IBAN { get; set; }

        [ForeignKey("IdUtilizador")]
        [ValidateNever]
        public virtual User user { get; set; }

        [ForeignKey("IdAdministradorValidador")]
        public virtual Administrador? AdministradorValidador { get; set; }
    }
}
