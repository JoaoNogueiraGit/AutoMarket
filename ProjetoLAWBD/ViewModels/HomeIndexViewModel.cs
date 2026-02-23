using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class HomeIndexViewModel {

        // Lista de carros para mostrar
        public List<Anuncio> VeiculosEmDestaque { get; set; }
        public List<Imagem> ImagensDosVeiculos { get; set; }

        // As listas para preencher as dropdowns
        public List<Marca> Marcas { get; set; } = new List<Marca>();
        public List<Modelo> Modelos { get; set; } = new List<Modelo>();
        public List<Caixa> Caixas { get; set; } = new List<Caixa>();
        public List<Combustivel> Combustiveis { get; set; } = new List<Combustivel>();
        public List<Cor> Cores { get; set; } = new List<Cor>();
        public List<Categoria> Categorias { get; set; } = new List<Categoria>();

        public decimal PrecoMaxDb { get; set; }


        // filtros
        public List<FiltroGuardado> FiltrosGuardados { get; set; } = new List<FiltroGuardado>();

        }
}
