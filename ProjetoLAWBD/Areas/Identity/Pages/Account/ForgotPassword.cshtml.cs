// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<User> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                // Verificação de Segurança:
                // Não revela se o utilizador não existe OU se o email não está confirmado.
                // Em ambos os casos, finge que funcionou para não dar pistas a atacantes.
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) {
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                

                // Gera um TOKEN de redefinição de password
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                // Cria o URL para a página "ResetPassword"
                var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code, Input.Email }, 
                protocol: Request.Scheme);

                
                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Recuperar Password - AutoMarket",
                    $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Recuperação de Password</h2>
                        <p>Recebemos um pedido para redefinir a sua senha.</p>
                        <p>Se não fez este pedido, ignore este email.</p>
                        <p>Para escolher uma nova senha, clique abaixo:</p>
                        <p>
                            <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' 
                               style='background-color: #dc3545; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                               Redefinir Password
                            </a>
                        </p>
                    </div>");

                // Redireciona para a página de confirmação
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
