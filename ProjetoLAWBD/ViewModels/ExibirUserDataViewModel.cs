using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.ViewModels
{
    public class ExibirUserDataViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime DataRegisto { get; set; }
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public Morada? MoradaUser { get; set; }
        public List<Contacto>? ContactosUser { get; set; }

    }
}
