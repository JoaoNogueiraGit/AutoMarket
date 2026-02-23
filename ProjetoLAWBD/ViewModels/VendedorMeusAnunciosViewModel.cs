using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class VendedorMeusAnunciosViewModel {

        // Os Números para os "Stat Cards"
        public int AnunciosAtivos { get; set; }
        public int AnunciosPausados { get; set; }
        public int VeiculosVendidos { get; set; }
        public int PedidosPendentes { get; set; }

        public int AnunciosArquivados { get; set; }
        // public int Visualizacoes { get; set; }

        // A Lista de Anúncios para mostrar em baixo
        public List<Anuncio> Anuncios { get; set; }

        public List<Imagem> ImagensDosVeiculos { get; set; }

        // Informação para as Tabs 
        public string StatusAtual { get; set; }


        // Propriedades para a paginação
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
