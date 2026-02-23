using System.ComponentModel.DataAnnotations;

namespace ProjetoLAWBD.ViewModels
{
    public class CriarAdminViewModel
    {
        [Required(ErrorMessage ="O nome de utilizador é obrigatório")]
        public string Username { get; set; }

        [Required(ErrorMessage = "O email é obrigatirio")]
        [EmailAddress(ErrorMessage = "Email invaildo")]
        public string Email { get; set; }

        [Required(ErrorMessage ="A password é obrigatoria")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Password")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem")]
        public string ConfirmPassword { get; set; }

        
    }
}
