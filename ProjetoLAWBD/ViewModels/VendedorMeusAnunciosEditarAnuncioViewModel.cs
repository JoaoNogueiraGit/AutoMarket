using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class VendedorMeusAnunciosEditarAnuncioViewModel {

        // O Anúncio (e o Veículo associado) que estamos a editar
        public Anuncio Anuncio { get; set; }

        // A lista de Imagens do Veículo (para as podermos ver/gerir)
        public List<Imagem> Imagens { get; set; }

        // As listas para preencher os <select> (dropdowns)
        public List<Marca> Marcas { get; set; }
        public List<Modelo> Modelos { get; set; }
        public List<Cor> Cores { get; set; }
        public List<Caixa> Caixas { get; set; }
        public List<Combustivel> Combustiveis { get; set; }
        public List<Categoria> Categorias { get; set; }
    }
}
