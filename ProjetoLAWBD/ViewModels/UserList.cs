namespace ProjetoLAWBD.ViewModels
{
    public class UserList
    {
        public string Id { get; set; }
        public string Nome { get; set; } // Username
        public string Email { get; set; }
        public string Tipo { get; set; } // Comprador / Vendedor / Administrador
        public string Estado {  get; set; } // Bloqueado / Ativo 
    }
}
