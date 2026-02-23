using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUtilizadores {  get; set; }
        public int VendedoresAtivos { get; set; }
        public int AnunciosAtivos { get; set; }
        public List<Vendedor> PedidosPendentes { get; set; }


        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
