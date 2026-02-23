namespace ProjetoLAWBD.ViewModels
{
    public class ItemAgendamentoViewModel
    {
        public int Id { get; set; }         
        public string Tipo { get; set; }     // "Reserva" ou "Visita"
        public string Estado { get; set; }   // "Pendente", "Aprovada", etc
        public DateTime DataPedido { get; set; }   
        public DateTime DataEvento { get; set; }

        
        public string NomeMarca { get; set; }
        public string NomeModelo { get; set; }

        public int IdAnuncio { get; set; }
    }

    public class AgendamentosUnificadosViewModel
    {
        public List<ItemAgendamentoViewModel> Ativas { get; set; }
        public List<ItemAgendamentoViewModel> Historico { get; set; }
    }
}
