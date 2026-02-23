namespace ProjetoLAWBD.ViewModels
{
    public class CompraViewModel
    {
        public int IdCompra { get; set; }
        public string NomeVeiculo { get; set; }
        public DateTime DataCompra { get; set; }
        public decimal ValorTotal { get; set; }
        public string ImagemUrl { get; set; }
    }
}
