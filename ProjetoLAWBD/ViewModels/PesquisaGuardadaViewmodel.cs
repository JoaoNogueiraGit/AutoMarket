namespace ProjetoLAWBD.ViewModels
{
    public class PesquisaGuardadaViewmodel
    {
        public int IdFiltro { get; set; }
        public string NomeFiltro { get; set; }
        public DateTime DataCriacao { get; set; }

        // Lista de strings para os badges (ex: "Marca: VW", "Preço: < 15k €")
        public List<string> Badges { get; set; } = new List<string>();
    }
}
