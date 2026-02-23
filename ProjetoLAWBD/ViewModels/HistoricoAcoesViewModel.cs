using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels
{
    public class HistoricoAcoesViewModel
    {
        public List<HistoricoDeAcoes> Historico { get; set; }

        public List<SelectListItem> AdminOpcoes { get; set; }
        public List<SelectListItem> TipoAcaoOpcoes { get; set; }

        public string FiltroAdmin { get; set; }
        public string FiltroTipoAcao { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
