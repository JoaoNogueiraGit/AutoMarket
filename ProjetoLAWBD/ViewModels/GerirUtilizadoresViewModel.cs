using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjetoLAWBD.ViewModels
{
    public class GerirUtilizadoresViewModel
    {
        public List<UserList> Utilizadores {  get; set; }

        public List<SelectListItem> Tipos { get; set; }
        public List<SelectListItem> Estados { get; set; }

        public string FiltroTipo { get; set; }
        public string FiltroEstado { get; set; }
        public GerirUtilizadoresViewModel()
        {
            Utilizadores = new List<UserList>();
        }

        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
