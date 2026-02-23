using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class AnuncioDetalhesViewModel {

        public Anuncio Anuncio { get; set; }
        public List<Imagem> Imagens { get; set; }

        public List<Contacto> ContactosVendedor { get; set; }
    }
}
