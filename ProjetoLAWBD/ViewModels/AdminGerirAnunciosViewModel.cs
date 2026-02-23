using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels
{
    public class AdminGerirAnunciosViewModel
    {
        public int Id { get; set; }
        public string Titulo {  get; set; }
        public string Vendedor { get; set; }
        public decimal Preco { get; set; }
        public string Estado { get; set; }

        public List<AdminGerirAnunciosViewModel> Anuncios { get; set; }

        public List<SelectListItem> Estados { get; set; }

        public string FiltroEstado { get; set; }

        public string FiltroPesquisa { get; set; }
       
    }
}
