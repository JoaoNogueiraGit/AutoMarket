using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.Models {
    public class FiltroGuardado {

        [Key]
        [Column("id_filtro")]
        public int IdFiltro { get; set; }

        [Required]
        [Column("id_utilizador")]
        public string IdUtilizador { get; set; }

        [Required]
        [Column("nome_filtro")]
        public string NomeFiltro { get; set; }

        [Required]
        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // --- NOVOS CAMPOS PARA TEXTO E GEO (ADICIONADOS) ---
        [Column("termo_pesquisa")]
        public string? Termo { get; set; } // Ex: "Teto Panorâmico"

        [Column("localizacao")]
        public string? Localizacao { get; set; } // Ex: "Lisboa + 30km"

        [Column("latitude")]
        public double? Latitude { get; set; }

        [Column("longitude")]
        public double? Longitude { get; set; }

        [Column("raio")]
        public int? Raio { get; set; }

        // --- CAMPOS EXISTENTES (RELACIONAMENTOS) ---
        [Column("id_marca")]
        public int? IdMarca { get; set; }

        [Column("id_modelo")]
        public int? IdModelo { get; set; }

        [Column("id_cor")]
        public int? IdCor { get; set; }

        [Column("id_caixa")]
        public int? IdCaixa { get; set; }

        [Column("id_combustivel")]
        public int? IdCombustivel { get; set; }

        [Column("id_categoria")]
        public int? IdCategoria { get; set; }

        // --- CAMPOS EXISTENTES (RANGES) ---
        [Column("ano_minimo")]
        public int? AnoMinimo { get; set; }

        [Column("ano_maximo")]
        public int? AnoMaximo { get; set; }

        [Column("km_maximo")]
        public int? KmMaximo { get; set; }

        [Column("preco_minimo")]
        public decimal? PrecoMinimo { get; set; }

        [Column("preco_maximo")]
        public decimal? PrecoMaximo { get; set; }

        // --- PROPRIEDADES DE NAVEGAÇÃO ---
        [ForeignKey("IdUtilizador")]
        public virtual User? User { get; set; }

        [ForeignKey("IdMarca")]
        public virtual Marca? Marca { get; set; }

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
    }
}