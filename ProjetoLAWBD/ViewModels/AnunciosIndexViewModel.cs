using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class AnunciosIndexViewModel {
        // ==========================================
        // 1. RESULTADOS DA PESQUISA
        // ==========================================
        public List<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

        // NOVO: Lista separada para as imagens, já que não estão dentro do objeto Veiculo
        public List<Imagem> ImagensDosVeiculos { get; set; }
        public int TotalAnuncios { get; set; }

        // ==========================================
        // 2. DADOS PARA OS DROPDOWNS (Listas de Opções)
        // ==========================================
        // Inicializamos com listas vazias para a View não "rebentar" se vierem a null
        public List<Marca> Marcas { get; set; } = new List<Marca>();
        public List<Modelo> Modelos { get; set; } = new List<Modelo>();
        public List<Combustivel> Combustiveis { get; set; } = new List<Combustivel>();
        public List<Caixa> Caixas { get; set; } = new List<Caixa>();

        public List<Cor> Cores { get; set; } = new List<Cor>();
        public List<Categoria> Categorias { get; set; } = new List<Categoria>();

        // ==========================================
        // 3. ESTADO DOS FILTROS (Inputs do Utilizador)
        // ==========================================
        // Estas propriedades guardam o que o user escreveu/escolheu para
        // manter os campos preenchidos após clicar em "Pesquisar".

        public string Termo { get; set; }        // Pesquisa livre 
        public string Localizacao { get; set; }  // Cidade (ex: "Lisboa")

        // IDs para chaves estrangeiras (Selects)
        public int? MarcaId { get; set; }
        public int? ModeloId { get; set; }
        public int? CombustivelId { get; set; }
        public int? CaixaId { get; set; }

        public int? CorId { get; set; }
        public int? CategoriaId { get; set; }

        // Intervalos (Ranges)
        public decimal? PrecoMin { get; set; }
        public decimal? PrecoMax { get; set; }

        public int? AnoDe { get; set; }
        public int? AnoAte { get; set; }

        public int? KmsAte { get; set; }

        // ==========================================
        // 4. CONTROLO DE LISTAGEM
        // ==========================================
        public string Ordenacao { get; set; } = "recente"; // Valor por defeito: Mais recentes primeiro

        public int PaginaAtual { get; set; } = 1;
        public int ItensPorPagina { get; set; } = 12;

        // Propriedade calculada útil para desenhar botões de paginação (1, 2, 3...)
        public int TotalPaginas => (int)Math.Ceiling((double)TotalAnuncios / ItensPorPagina);

        public bool TemPaginaAnterior => PaginaAtual > 1;
        public bool TemProximaPagina => PaginaAtual < TotalPaginas;


        // Campos para a pesquisa por Raio
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Raio { get; set; } // Em Km


        // estado do filtro
        public int? IdFiltroAtivo { get; set; } // O ID do filtro que foi carregado (se houver)
        public string? NomeFiltroAtivo { get; set; } // O nome dele (para mostrar no modal)


        public decimal PrecoMaxDb { get; set; } // Para definir o valor máximo do slider de preço no front-end
}
}