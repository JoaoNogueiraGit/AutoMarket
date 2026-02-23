using ProjetoLAWBD.Models;
using System.Collections.Generic;

namespace ProjetoLAWBD.ViewModels {
    public class VendedorMeusVeiculosViewModel {
        
        // Lista especial que combina o Veículo com o seu status
        public List<VeiculoComStatus> Veiculos { get; set; }

        public List<Imagem> ImagensDoVeiculos { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

    }

    // Uma classe "ajudante" que representa um item na lista
    public class VeiculoComStatus {
        public Veiculo Veiculo { get; set; }
        public string StatusDoAnuncio { get; set; }
        public int? AnuncioId { get; set; } // Para o link "Ver Anúncio"
    }

}
