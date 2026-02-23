using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.Filters {
    public class CheckVendedorPendente : IAsyncActionFilter {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CheckVendedorPendente(ApplicationDbContext context, UserManager<User> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            var user = context.HttpContext.User;

            if (user.Identity.IsAuthenticated && user.IsInRole("Vendedor")) {
                var userId = _userManager.GetUserId(user);
                var vendedor = await _context.Vendedores.FindAsync(userId);

                // Se o Vendedor existe e está Pendente
                if (vendedor != null && vendedor.EstadoValidacao == "Pendente") {
                    var controllerName = context.RouteData.Values["controller"].ToString();
                    var actionName = context.RouteData.Values["action"].ToString();
                    var areaName = context.RouteData.Values["area"]?.ToString();

                    // Permitir Logout e gestão de conta base
                    if (areaName == "Identity" && controllerName == "Account") {
                        await next();
                        return;
                    }

                    // VERIFICAR SE JÁ PREENCHEU OS DADOS OBRIGATÓRIOS
                    // (Consideramos que se tem NIF e NomeFaturacao, já preencheu o básico)
                    bool dadosPreenchidos = !string.IsNullOrEmpty(vendedor.NIF) &&
                                            !string.IsNullOrEmpty(vendedor.NomeFaturacao);

                    if (dadosPreenchidos) {
                        // === CENÁRIO 1: DADOS PREENCHIDOS, MAS NÃO APROVADO ===
                        // Deve estar na página "AguardandoAprovacao"
                        // Permitimos também ir ao "DadosFaturacao" caso queira corrigir algo.

                        bool paginaPermitida = (controllerName == "Vendedor" &&
                                               (actionName == "AguardandoAprovacao" || actionName == "DadosFaturacao"));

                        if (!paginaPermitida) {
                            context.Result = new RedirectToActionResult("AguardandoAprovacao", "Vendedor", null);
                            return;
                        }
                    }
                    else {
                        // === CENÁRIO 2: DADOS EM FALTA ===
                        // Deve estar obrigatoriamente na página "DadosFaturacao"

                        if (controllerName != "Vendedor" || actionName != "DadosFaturacao") {
                            var controller = context.Controller as Controller;
                            if (controller != null) {
                                controller.TempData["InfoMessage"] = "Preencha os dados obrigatórios para prosseguir.";
                            }
                            context.Result = new RedirectToActionResult("DadosFaturacao", "Vendedor", null);
                            return;
                        }
                    }
                }
            }

            await next();
        }
    }
}