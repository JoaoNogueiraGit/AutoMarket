using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class VendedorAdicionarVeiculoViewModel {

        // O novo veículo
        public Veiculo Veiculo {  get; set; }

        // A lista de ficheiros que vêm do upload
        public List<string> FicheirosImagem { get; set; }


        // As listas para preencher as dropdowns
        public List<Marca> Marcas { get; set; }
        public List<Modelo> Modelos { get; set; }
        public List<Caixa> Caixas { get; set; }
        public List<Combustivel> Combustiveis { get; set; }
        public List<Cor> Cores { get; set; }
        public List<Categoria> Categorias { get; set; }
    }
}
