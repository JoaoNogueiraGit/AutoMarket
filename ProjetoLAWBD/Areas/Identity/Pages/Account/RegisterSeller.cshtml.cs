// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
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
using SQLitePCL;

namespace ProjetoLAWBD.Areas.Identity.Pages.Account {
    public class RegisterSellerModel : PageModel {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterSellerModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IReCAPTCHAService _recaptchaService;
        public RegisterSellerModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterSellerModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
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

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 

        public bool IsLoggedInUser { get; set; } = false;

        public enum TypeSeller {
            [Display(Name = "Sou Comprador")]
            Particular = 0,

            [Display(Name = "Sou Vendedor")]
            Empresa = 1
        }

        public class InputModel {

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }


            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Tem de escolher um tipo de vendedor.")]
            [Display(Name = "Tipo de Vendedor")]
            public TypeSeller TypeSeller { get; set; }

            public DateTime DataRegisto { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null) {

            if(_signInManager.IsSignedIn(User)) {

                // Is logged
                IsLoggedInUser = true;

                // View is gonna present a message saying "must logout"
                return Page();
            }

            // If not logged, continue as normal
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {


            // If a logged user tries to submit form, redirects to homepage
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
                        
                        // Atribuir role de "Vendedor"
                        await _userManager.AddToRoleAsync(user, "Vendedor");

                        // Criar a entrada na tabela Vendedor
                        var novoVendedor = new Vendedor {
                            IdUtilizador = user.Id,
                            NIF = "000000000",           // NIF Temporário
                            TipoVendedor = Input.TypeSeller.ToString(), 
                            EstadoValidacao = "Pendente" // Começa como pendente
                                                         // O resto (IBAN, etc.) fica NULL
                        };
                        _context.Vendedores.Add(novoVendedor);

                        // Guardar o Vendedor na base de dados
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Utilizador {user.Id} especializado como Vendedor.");

                        // ----------------------------------------------
                        // --- ADICIONE ESTAS 3 LINHAS DE CÓDIGO AQUI ---
                        //_logger.LogWarning($"--- HACK DE TESTE: A bloquear o utilizador {user.Email} ---");
                        //var dataBloqueio = DateTimeOffset.UtcNow.AddYears(100); // Define o bloqueio para 100 anos
                        //await _userManager.SetLockoutEndDateAsync(user, dataBloqueio);
                        //// --- FIM DO HACK DE TESTE ---
                        // ----------------------------------------------

                    } catch (Exception ex) {
                        // Falha ao criar especializacao
                        _logger.LogError(ex, $"ERRO CRÍTICO: Falha ao especializar o utilizador {user.Id}.");

                        // Apagar o utilizador que acabámos de criar
                        await _userManager.DeleteAsync(user);

                        ModelState.AddModelError(string.Empty, "Ocorreu um erro ao finalizar o seu registo. Tente novamente.");
                        return Page();
                    }

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

            // If we got this far, something failed, redisplay form
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
