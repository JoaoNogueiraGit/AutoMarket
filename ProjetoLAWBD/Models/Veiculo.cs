using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class Veiculo {

        [Key]
        [Column("id_veiculo")]
        public int IdVeiculo { get; set; }

        [Required]
        [Column("id_vendedor")]
        public string IdVendedor { get; set; }

        [Required]
        [Column("id_modelo")]
        public int IdModelo { get; set; }

        [Required]
        [Column("id_cor")]
        public int IdCor { get; set; }

        [Required]
        [Column("id_caixa")]
        public int IdCaixa { get; set; }
       
        [Required]
        [Column("id_combustivel")]
        public int IdCombustivel { get; set; }

        [Required]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Required]
        [Column("potencia_cv")]
        public int PotenciaCV { get; set; }

        [Column("cilindrada_cc")]
        public int? CilindradaCC { get; set; }

        [Required]
        [Column("num_lugares")]
        public int NumLugares { get; set; }

        [Required]
        [Column("num_portas")]
        public int NumPortas { get; set; }

        [Required]
        [Column("data_matricula")]
        public DateTime DataMatricula { get; set; }

        [Required]
        [Column("km_total")]
        public int KmTotal { get; set; }

        [ForeignKey("IdVendedor")]
        public virtual Vendedor? Vendedor { get; set; }

        [ForeignKey("IdModelo")]
        public virtual Modelo? Modelo { get; set; }

        [ForeignKey("IdCor")]
        public virtual Cor? Cor { get; set; }

        [ForeignKey("IdCaixa")]
        public virtual Caixa? Caixa { get; set; }

        [ForeignKey("IdCombustivel")]
        public virtual Combustivel? Combustivel { get; set; }

        [ForeignKey("IdCategoria")]
        public virtual Categoria? Categoria { get; set; }

        public bool Arquivado { get; set; } = false;
    }
}
