using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoLAWBD.ViewModels {
    public class CheckoutViewModel {
        // --- DADOS DO ANÚNCIO (Read-only) ---
        public int AnuncioId { get; set; }
        public string TituloAnuncio { get; set; }
        public string ImagemUrl { get; set; }
        public decimal Preco { get; set; }
        public string MarcaModelo { get; set; }

        // --- DADOS PESSOAIS ---
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome na Fatura")]
        public string NomeFatura { get; set; }

        // --- MORADA (Igual ao teu Model) ---
        [Required(ErrorMessage = "A rua e número são obrigatórios.")]
        [Display(Name = "Rua e Número")]
        public string RuaENumero { get; set; } // Mudado de 'Rua' para 'RuaENumero'

        [Required(ErrorMessage = "O código postal é obrigatório.")]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Formato: 0000-000")]
        [Display(Name = "Código Postal")]
        public string CodigoPostal { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O distrito é obrigatório.")]
        public string Distrito { get; set; } // Adicionado o Distrito


        

        // --- PAGAMENTO ---
        [Required(ErrorMessage = "Selecione um método de pagamento.")]
        public string MetodoPagamento { get; set; }
    }
}