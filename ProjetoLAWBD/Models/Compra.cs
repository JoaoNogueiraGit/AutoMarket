using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.Models {
    public class Compra {

        [Key]
        [Column("id_compra")]
        public int IdCompra { get; set; }

        [Required]
        [Column("id_comprador")]
        public string IdComprador { get; set; }

        [Required]
        [Column("id_anuncio")]
        public int IdAnuncio { get; set; }

        [Required]
        [Column("data_compra")]
        public DateTime DataCompra { get; set; }

        [Required]
        [Column("estado_pagamento")]
        public string EstadoPagamento { get; set; }

        [Required]
        [Column("valor_total")]
        public decimal ValorTotal { get; set; }

        [ForeignKey("IdComprador")]
        public virtual Comprador Comprador { get; set; }

        [ForeignKey("IdAnuncio")]
        public virtual Anuncio Anuncio { get; set; }


        // Faturação
        public string NumeroFatura { get; set; } // Ex: FAT-2025/001

        // Dados fiscais estáticos (para a fatura não mudar se o user mudar de morada depois)
        public string? NomeCompradorSnapshot { get; set; }
        public string? MoradaCompradorSnapshot { get; set; }

        // Dados Faturação Vendedor
        public string? NomeFaturacao { get; set; }

        public string? RuaFaturacao { get; set; }

        public string? CPFaturacao { get; set; }

        public string? CidadeFaturacao { get; set; }

        public string? NIFVendedor { get; set; }

        public string? IBAN { get; set; }
    }
}
