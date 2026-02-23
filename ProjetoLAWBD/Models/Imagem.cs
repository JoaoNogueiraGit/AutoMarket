using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class Imagem {

        [Key]
        [Column("id_imagem")]
        public int IdImagem { get; set; }

        [Required]
        [Column("id_veiculo")]
        public int IdVeiculo { get; set; }

        [Required]
        [Column("caminho_ficheiro")]
        public string CaminhoFicheiro { get; set; }

        [Required]
        [Column("is_capa")]
        public bool IsCapa { get; set; }

        [Required]
        [Column("ordem_exibicao")]
        public int OrdemExibicao { get; set; }

        [ForeignKey("IdVeiculo")]
        public virtual Veiculo Veiculo { get; set; }
    }
}
