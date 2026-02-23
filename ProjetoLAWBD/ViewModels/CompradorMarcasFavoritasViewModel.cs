using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels
{
    public class CompradorMarcasFavoritasViewModel
    {
        public List<SelectListItem> TodasMarcas { get; set; }
        public List<MarcaFavoritaComprador> MinhasMarcas { get; set; }
    }
}
