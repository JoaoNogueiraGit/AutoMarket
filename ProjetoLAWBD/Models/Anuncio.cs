using ProjetoLAWBD.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class Anuncio {

        [Key]
        [Column("id_anuncio")]
        public int IdAnuncio { get; set; }

        [Required]
        [Column("id_vendedor")]
        public string IdVendedor { get; set; }

        [Required]
        [Column("id_veiculo")]
        public int IdVeiculo { get; set; }

        [Required]
        [Column("titulo")]
        public string Titulo { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Required]
        [Column("preco")]
        public decimal Preco { get; set; }

        [Required]
        [LocalizacaoValida(ErrorMessage = "Essa cidade não existe na nossa lista.")]
        [Column("localizacao_cidade")]
        public string LocalizacaoCidade { get; set; }

        [Required]
        [Column("estado")]
        public string Estado { get; set; }

        [Required]
        [Column("data_publicacao")]
        public DateTime DataPublicacao { get; set; }

        [ForeignKey("IdVendedor")]
        public virtual Vendedor Vendedor { get; set; }

        [ForeignKey("IdVeiculo")]
        public virtual Veiculo Veiculo { get; set; }
    }
}
