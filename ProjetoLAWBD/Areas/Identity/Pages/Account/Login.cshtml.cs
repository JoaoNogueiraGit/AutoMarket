// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.Services;
using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Data;

namespace ProjetoLAWBD.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IReCAPTCHAService _recaptchaService;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger, IReCAPTCHAService recaptchaService, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _recaptchaService = recaptchaService;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        public bool IsAccountLocked { get; set; } = false;

        public string MotivoBloqueio { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress]
            [Display(Name = "Email")] 
            public string Email { get; set; }

            
            [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")] 
            public string Password { get; set; }

            
            [Display(Name = "Lembrar-me")] 
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {

            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            // Validação recaptcha
            var recaptchaToken = Request.Form["g-recaptcha-response"];
            if (!await _recaptchaService.IsValid(recaptchaToken)) {
                // Se falhar, recarrega a página com um erro
                ModelState.AddModelError(string.Empty, "A verificação reCAPTCHA falhou. Por favor, tente novamente.");
                return Page();
            }
            // Fim validação recaptcha

            if (ModelState.IsValid) {
                // Procurar o utilizador pelo email
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                if (user == null) {
                    ModelState.AddModelError(string.Empty, "Email ou password inválidos.");
                    return Page();
                }

                // Fazer login usando o UserName do utilizador encontrado
                var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded) {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor) {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut) {
                    _logger.LogWarning("User account locked out.");

                    // Ativa a flag para a View
                    IsAccountLocked = true;

                    var bloqueioInfo = await _context.Bloqueios
                        .Where(b => b.IdUtilizadorAlvo == user.Id)
                        .OrderByDescending(b => b.DataBloqueio)
                        .FirstOrDefaultAsync();

                    MotivoBloqueio = bloqueioInfo?.Motivo ?? "Comportamento indevido ou violação dos termos.";

                    // Fica na mesma página (em vez de redirecionar)
                    return Page();
                }
                else {
                    ModelState.AddModelError(string.Empty, "Email ou password inválidos.");
                    return Page();
                }
            }

            // Se chegou aqui, algo falhou, reexibe o formulário
            return Page();
        }

    }
}
