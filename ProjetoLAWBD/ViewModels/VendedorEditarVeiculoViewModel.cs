using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class VendedorEditarVeiculoViewModel {

        public Veiculo Veiculo { get; set; }

        // Listas para Dropdowns
        public List<Marca> Marcas { get; set; }
        public List<Modelo> Modelos { get; set; }
        public List<Cor> Cores { get; set; }
        public List<Caixa> Caixas { get; set; }
        public List<Combustivel> Combustiveis { get; set; }
        public List<Categoria> Categorias { get; set; }

        // --- PARA O FILEPOND ---

        // Imagens que já existem na BD (para mostrar no load)
        public List<Imagem> ImagensExistentes { get; set; } = new List<Imagem>();

        // Novas imagens que o user adicionar (Base64 vindo do FilePond)
        public List<string> FicheirosImagem { get; set; }

        // IDs das imagens antigas que o user removeu (separados por vírgula)
        public string? idsImagensParaRemover { get; set; }
    }
}
