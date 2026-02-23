using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.Areas.Identity.Pages.Account.Manage {
    public class IndexModel : PageModel {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context; // <--- INJETA O DBCONTEXT

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext context) {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }
        public string Email { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        // Lista para mostrar na tabela
        public List<Contacto> ListaContactos { get; set; }

        public class InputModel {
            [Display(Name = "Nome Completo")]
            public string NomeCompleto { get; set; }

            // Campos para ADICIONAR novo contacto
            [Display(Name = "Tipo de Contacto")]
            public string NovoTipoContacto { get; set; }

            [Display(Name = "Valor (Nº ou Email)")]
            public string NovoValorContacto { get; set; }
        }

        private async Task LoadAsync(User user) {
            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);

            Username = userName;
            Email = email;

            // Carregar os contactos da base de dados
            ListaContactos = await _context.Contactos
                .Where(c => c.IdUtilizador == user.Id)
                .ToListAsync();

            // Inicializar o Input se for null (para não dar erro no primeiro load)
            if (Input == null) {
                Input = new InputModel {
                    NomeCompleto = user.NomeCompleto
                };
            }
        }

        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        // --- HANDLER 1: Guardar Perfil (Nome) ---
        public async Task<IActionResult> OnPostAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Guardar Nome Completo
            if (Input.NomeCompleto != user.NomeCompleto) {
                user.NomeCompleto = Input.NomeCompleto;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded) {
                    StatusMessage = "Erro inesperado ao tentar guardar o Nome Completo.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "O seu perfil foi atualizado";
            return RedirectToPage();
        }

        // --- HANDLER 2: Adicionar Contacto ---
        public async Task<IActionResult> OnPostAdicionarContactoAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(Input.NovoTipoContacto) && !string.IsNullOrEmpty(Input.NovoValorContacto)) {
                var novoContacto = new Contacto {
                    IdUtilizador = user.Id,
                    TipoContacto = Input.NovoTipoContacto,
                    ValorContacto = Input.NovoValorContacto
                };

                _context.Contactos.Add(novoContacto);
                await _context.SaveChangesAsync();
                StatusMessage = "Novo contacto adicionado com sucesso.";
            }
            else {
                StatusMessage = "Erro: Preencha o tipo e o valor do contacto.";
            }

            return RedirectToPage();
        }

        // --- HANDLER 3: Remover Contacto ---
        public async Task<IActionResult> OnPostRemoverContactoAsync(int idContacto) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var contacto = await _context.Contactos.FindAsync(idContacto);

            // Segurança: Garantir que o contacto pertence mesmo a este utilizador
            if (contacto != null && contacto.IdUtilizador == user.Id) {
                _context.Contactos.Remove(contacto);
                await _context.SaveChangesAsync();
                StatusMessage = "Contacto removido.";
            }

            return RedirectToPage();
        }
    }
}