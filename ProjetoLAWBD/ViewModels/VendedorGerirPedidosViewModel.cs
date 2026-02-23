using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels {
    public class VendedorGerirPedidosViewModel {

        // Contagens para as Tabs
        public int Pendentes { get; set; }
        public int Aprovados { get; set; }
        public int Historico { get; set; } // (Rejeitados, Expirados, Concluídos, etc.)

        // Listas de Pedidos
        public List<Visita> PedidosVisita { get; set; }
        public List<Reserva> PedidosReserva { get; set; }

        // A Tab que está ativa
        public string StatusAtual { get; set; }
    }
}
