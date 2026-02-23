using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class CriarAnuncioViewModel {

        public Veiculo Veiculo { get; set; }
        public Anuncio Anuncio { get; set; }

        public List<Imagem> Imagens { get; set; }
    }
}
