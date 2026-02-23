#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.Services;

namespace ProjetoLAWBD.Areas.Identity.Pages.Account {
    // O nome da classe deve corresponder ao nome do ficheiro
    public class RegisterBuyerModel : PageModel {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterBuyerModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IReCAPTCHAService _recaptchaService;


        public RegisterBuyerModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterBuyerModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            IReCAPTCHAService recaptchaService) {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _recaptchaService = recaptchaService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // Propriedade para a View saber se o utilizador está logado
        public bool IsLoggedInUser { get; set; } = false;


        public class InputModel {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Palavra-passe")]
            [Compare("Password", ErrorMessage = "A palavra-passe e a confirmação não correspondem.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "O nome de utilizador é obrigatório.")]
            [Display(Name = "Nome de Utilizador")]
            public string UserName { get; set; }

            public DateTime DataCriacao { get; set; }

        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null) {
            // Lógica para verificar se já está logado
            if (_signInManager.IsSignedIn(User)) {
                IsLoggedInUser = true;
                return Page();
            }

            // Se não estiver logado, continua normal
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            // Proteção: Se um utilizador logado submeter este formulário, redireciona
            if (_signInManager.IsSignedIn(User)) {
                return RedirectToAction("Index", "Home");
            }

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var recaptchaToken = Request.Form["g-recaptcha-response"];
            if (!await _recaptchaService.IsValid(recaptchaToken)) {
                ModelState.AddModelError(string.Empty, "A verificação reCAPTCHA falhou. Por favor, tente novamente.");
                // Recarrega os logins externos antes de devolver a página
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            if (ModelState.IsValid) {
                var user = CreateUser();

                user.DataRegisto = DateTime.UtcNow;
                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded) {
                    _logger.LogInformation("User created a new account with password.");

                    try {
                        

                        // Atribuir role de "Comprador"
                        await _userManager.AddToRoleAsync(user, "Comprador");

                        // Criar a entrada na tabela Comprador
                        var novoComprador = new Comprador { IdUtilizador = user.Id };
                        _context.Compradores.Add(novoComprador);

                        // Guardar na base de dados
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Utilizador {user.Id} especializado como Comprador.");
                    } catch (Exception ex) {
                        // Se falhar, apaga o Utilizador do Identity (rollback)
                        _logger.LogError(ex, $"ERRO CRÍTICO: Falha ao especializar o utilizador {user.Id} como Comprador.");
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError(string.Empty, "Ocorreu um erro ao finalizar o seu registo. Tente novamente.");
                        return Page();
                    }

                    // Lógica Padrão de Confirmação de Email
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                                        
                    await _emailSender.SendEmailAsync(Input.Email, "Confirme o seu email - AutoMarket",
                        $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px;'>
                            <h2>Bem-vindo ao AutoMarket!</h2>
                            <p>Obrigado por criar uma conta.</p>
                            <p>Por favor confirme o seu email para ativar a conta:</p>
                            <p>
                                <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' 
                                   style='background-color: #000; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                                   Confirmar Email Agora
                                </a>
                            </p>
                        </div>");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount) {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                    }
                    else {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se algo falhou, mostra o formulário novamente
            return Page();
        }

        private User CreateUser() {
            try {
                return Activator.CreateInstance<User>();
            } catch {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore() {
            if (!_userManager.SupportsUserEmail) {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}