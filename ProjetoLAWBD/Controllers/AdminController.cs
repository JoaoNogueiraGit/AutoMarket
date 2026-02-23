using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.ViewModels; 
using System.Security.Claims;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjetoLAWBD.Controllers
{
    [Authorize(Roles ="Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // No AdminController.cs

        [HttpGet]
        public async Task<IActionResult> Index(int pagina = 1)
        {
            var viewModel = await GetDashboardViewModel(pagina);

            // Se for um pedido AJAX, devolvemos apenas a Partial View da tabela
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PedidosPendentesTable", viewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprovar(string id, int pagina = 1)
        {
            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor != null)
            {
                vendedor.EstadoValidacao = "Aprovado";
                vendedor.DataValidacao = DateTime.UtcNow;
                vendedor.IdAdministradorValidador = _userManager.GetUserId(User);
                _context.Vendedores.Update(vendedor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vendedor aprovado.";
            }

            // Após a ação, recarregamos o ViewModel e devolvemos a Partial atualizada
            var viewModel = await GetDashboardViewModel(pagina);
            return PartialView("_PedidosPendentesTable", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rejeitar(string id, int pagina = 1)
        {
            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor != null)
            {
                vendedor.EstadoValidacao = "Rejeitado";
                vendedor.DataValidacao = DateTime.UtcNow;
                vendedor.IdAdministradorValidador = _userManager.GetUserId(User);
                _context.Vendedores.Update(vendedor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pedido rejeitado.";
            }

            var viewModel = await GetDashboardViewModel(pagina);
            return PartialView("_PedidosPendentesTable", viewModel);
        }

        // Método auxiliar para evitar repetição de código
        private async Task<AdminDashboardViewModel> GetDashboardViewModel(int pagina)
        {
            int itensPorPagina = 5;
            var query = _context.Vendedores.Include(v => v.user).Where(v => v.EstadoValidacao == "Pendente");

            int totalItems = await query.CountAsync();
            var pendentes = await query.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina).ToListAsync();

            return new AdminDashboardViewModel
            {
                TotalUtilizadores = await _context.Users.CountAsync(),
                VendedoresAtivos = await _context.Vendedores.CountAsync(v => v.EstadoValidacao == "Aprovado"),
                AnunciosAtivos = await _context.Anuncios.CountAsync(a => a.Estado == "Ativo"),
                PedidosPendentes = pendentes,
                PaginaAtual = pagina,
                TotalPaginas = (int)Math.Ceiling(totalItems / (double)itensPorPagina)
            };
        }


        [HttpGet]
        public async Task<IActionResult> GerirUtilizadores(string filtroTipo, string filtroEstado, int pagina = 1)
        {
            var viewmodel = await PrepararGerirUtilizadoresViewModel(filtroTipo, filtroEstado, pagina);

            // Se o pedido for Ajax (vindo dos filtros ou paginação), devolve apenas a Partial
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UtilizadoresTable", viewmodel);
            }

            return View(viewmodel);
        }


        private async Task<GerirUtilizadoresViewModel> PrepararGerirUtilizadoresViewModel(string filtroTipo, string filtroEstado, int pagina)
        {
            int itensPorPagina = 10;
            var agora = DateTimeOffset.UtcNow;

            var query = _context.Users.Select(u => new UserList
            {
                Id = u.Id,
                Nome = u.UserName,
                Email = u.Email,
                Tipo = _context.Vendedores.Any(v => v.IdUtilizador == u.Id) ? "Vendedor" :
                       _context.Compradores.Any(c => c.IdUtilizador == u.Id) ? "Comprador" :
                       _context.Administradores.Any(a => a.IdUtilizador == u.Id) ? "Administrador" : "Indefinido",
                Estado = (u.LockoutEnd.HasValue && u.LockoutEnd > agora) ? "Bloqueado" : "Ativo"
            });

            // Filtros
            if (!string.IsNullOrEmpty(filtroTipo))
                query = query.Where(u => u.Tipo == filtroTipo);

            if (!string.IsNullOrEmpty(filtroEstado))
                query = query.Where(u => u.Estado == filtroEstado);

            int totalItens = await query.CountAsync();

            var itensPaginados = await query
                .OrderBy(u => u.Nome)
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToListAsync();

            // RETORNO COMPLETO COM AS LISTAS
            return new GerirUtilizadoresViewModel
            {
                Utilizadores = itensPaginados,
                PaginaAtual = pagina,
                TotalPaginas = (int)Math.Ceiling(totalItens / (double)itensPorPagina),
                FiltroTipo = filtroTipo,
                FiltroEstado = filtroEstado,

                Tipos = new List<SelectListItem> {
                    new SelectListItem { Value = "Vendedor", Text = "Vendedor" },
                    new SelectListItem { Value = "Comprador", Text = "Comprador" },
                    new SelectListItem { Value = "Administrador", Text = "Administrador" }
                },
                Estados = new List<SelectListItem> {
                    new SelectListItem { Value = "Ativo", Text = "Ativo" },
                    new SelectListItem { Value = "Bloqueado", Text = "Bloqueado" }
                }
            };
        }

        [HttpGet]
        public async Task<IActionResult> GerirAnuncios(string filtroEstado, string filtroPesquisa)
        {
            var query = _context.Anuncios
                .Include(a => a.Vendedor)
                    .ThenInclude(v => v.user)
                .OrderByDescending(a => a.DataPublicacao)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroEstado))
                query = query.Where(a => a.Estado == filtroEstado);

            if (!string.IsNullOrEmpty(filtroPesquisa))
                query = query.Where(a => a.Titulo.Contains(filtroPesquisa) || a.IdAnuncio.ToString() == filtroPesquisa);

            var anunciosDB = await query.ToListAsync();

            // Mapeia para a lista de itens que a View espera
            var listaVM = anunciosDB.Select(a => new AdminGerirAnunciosViewModel
            {
                Id = a.IdAnuncio,
                Titulo = a.Titulo,
                Vendedor = a.Vendedor?.user?.UserName ?? "Vendedor Apagado",
                Preco = a.Preco,
                Estado = a.Estado
            }).ToList();

            // Cria o viewmodel da página com as propriedades usadas pela View
            var pageModel = new AdminGerirAnunciosViewModel
            {
                Anuncios = listaVM,
                FiltroEstado = filtroEstado,
                FiltroPesquisa = filtroPesquisa,
                Estados = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Todos os Estados" },
                    new SelectListItem { Value = "Ativo", Text = "Ativo" },
                    new SelectListItem { Value = "Pausado", Text = "Pausado" },
                    new SelectListItem { Value = "Removido", Text = "Removido" }
                }
            };

            return View(pageModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PausarAnuncio(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);

            if (anuncio == null) return NotFound();

            anuncio.Estado = "Pausado";

            _context.Anuncios.Update(anuncio);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Anuncio {anuncio.Titulo ?? "ID: " + id} pausado.";

            return RedirectToAction(nameof(GerirAnuncios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtivarAnuncio(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);

            if (anuncio == null) return NotFound();

            anuncio.Estado = "Ativo";

            _context.Anuncios.Update(anuncio);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Anuncio {anuncio.Titulo ?? "ID: " + id} Ativado.";

            return RedirectToAction(nameof(GerirAnuncios));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverAnuncio(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);

            if (anuncio == null) return NotFound();

            anuncio.Estado = "Arquivado";

            _context.Anuncios.Update(anuncio);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Anuncio {anuncio.Titulo ?? "ID: " + id} removido.";

            return RedirectToAction(nameof(GerirAnuncios));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bloquear(string id, string motivo, string filtroTipo, string filtroEstado, int pagina = 1)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // 1. O UserManager já faz o Update e o SaveChanges na tabela de Users
            var resultado = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

            if (resultado.Succeeded)
            {
                // 2. Criamos o registo de bloqueio e histórico
                _context.Bloqueios.Add(new Bloqueio
                {
                    IdUtilizadorAlvo = user.Id,
                    IdAdministrador = _userManager.GetUserId(User),
                    DataBloqueio = DateTime.UtcNow,
                    Motivo = motivo
                });

                _context.HistoricoDeAcoes.Add(new HistoricoDeAcoes
                {
                    IdAdministradorAutor = _userManager.GetUserId(User),
                    DataAcao = DateTime.UtcNow,
                    TipoAcao = "Bloqueio de Utilizador",
                    Descricao = motivo,
                    EntidadeAlvo = "Utilizador",
                    IdEntidadeAlvo = user.Id
                });

                // 3. Gravamos APENAS o Bloco e o Histórico. 
                // Não faças _context.Update(user) aqui!
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Utilizador {user.UserName} bloqueado.";
            }

            var model = await PrepararGerirUtilizadoresViewModel(filtroTipo, filtroEstado, pagina);
            return PartialView("_UtilizadoresTable", model);
        }

        [HttpPost]
        public async Task<IActionResult> Desbloquear(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            try
            {
                // 1. Remove a data de bloqueio
                await _userManager.SetLockoutEndDateAsync(user, null);

                // 2. Adiciona o histórico (exemplo do seu código)
                var log = new HistoricoDeAcoes
                {
                    IdAdministradorAutor = _userManager.GetUserId(User),
                    DataAcao = DateTime.UtcNow,
                    TipoAcao = "Desbloqueio de Utilizador",
                    Descricao = $"Utilizador {user.UserName} desbloqueado.",
                    EntidadeAlvo = "Utilizador",
                    IdEntidadeAlvo= user.Id
                };
                _context.HistoricoDeAcoes.Add(log);

                // 3. Salva tudo atomicamente
                await _context.SaveChangesAsync();

                TempData["Success"] = "Utilizador desbloqueado com sucesso.";
            }
            catch (DbUpdateConcurrencyException)
            {
                // Se cair aqui, significa que os dados mudaram no banco enquanto você tentava salvar
                ModelState.AddModelError("", "O registro foi atualizado por outro usuário. Por favor, recarregue a página.");
                return View(user);
            }

            return RedirectToAction(nameof(GerirUtilizadores));
        }


        [HttpGet]
        public IActionResult CriarAdministrador()
        {
            return View(new CriarAdminViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarAdministrador(CriarAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    DataRegisto = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Administrador");

                    var adminEntry = new Administrador
                    {
                        IdUtilizador = user.Id,
                        IdAdministradorCriador = _userManager.GetUserId(User)
                    };

                    _context.Administradores.Add(adminEntry);

                    var log = new HistoricoDeAcoes
                    {
                        IdAdministradorAutor = _userManager.GetUserId(User),
                        DataAcao = DateTime.UtcNow,
                        TipoAcao = "Criação de Administrador",
                        Descricao = $"Administrador {user.UserName} criado.",
                        EntidadeAlvo = "Utilizador",
                        IdEntidadeAlvo = user.Id
                    };

                    _context.HistoricoDeAcoes.Add(log);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SucessMessage"] = $"Administrador {user.UserName} criado com sucesso!";
                    return RedirectToAction(nameof(GerirUtilizadores));
                }

                await transaction.RollbackAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado. Tente novamente.");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetUserData(string id)
        {
            if (id == null) return NotFound(); 

            var user = await _context.Users
                .Include(u => u.Morada)
                .Include(u => u.Contactos)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var viewmodel = new ExibirUserDataViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                DataRegisto = user.DataRegisto,
                MoradaUser = user.Morada,
                ContactosUser = user.Contactos.ToList(),
                Tipo = await _context.Vendedores.AnyAsync(v => v.IdUtilizador == id) ? "Vendedor" : "Comprador",
                Estado = (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow) ? "Bloqueado" : "Ativo"
            };

            return View(viewmodel);
        }

        [HttpGet]
        public async Task<IActionResult> HistoricoAcoes(string filtroAcao, string filtroAdmin, int pagina = 1)
        {

            int itensPorPagina = 10;
            
            var query = _context.HistoricoDeAcoes
                                .Include(h => h.AdministradorAutor)
                                    .ThenInclude(a => a.user)
                                .OrderByDescending(h => h.DataAcao)
                                .AsQueryable();
            //Filtros
            if (!string.IsNullOrEmpty(filtroAdmin))
            {
                query = query.Where(h => h.IdAdministradorAutor == filtroAdmin);
            }

            if (!string.IsNullOrEmpty(filtroAcao))
            {
                query = query.Where(h => h.TipoAcao == filtroAcao);
            }

            int totalItens = await query.CountAsync();

            var listaHistoricoPaginada = await query
                        .OrderByDescending(h => h.DataAcao)
                        .Skip((pagina - 1) * itensPorPagina)
                        .Take(itensPorPagina)
                        .ToListAsync();

            var ListaAdmin = await _context.Administradores
                    .Include(a => a.user)
                    .Select(a => new SelectListItem
                    {
                        Value = a.IdUtilizador,
                        Text = a.user.UserName
                    })
                    .ToListAsync();
            ListaAdmin.Insert(0, new SelectListItem { Value = "", Text = "Todos os Administradores" });

            var ListaTipos = await _context.HistoricoDeAcoes
                    .Select(h => h.TipoAcao)
                    .Distinct()
                    .Select(t => new SelectListItem { Value = t, Text = t })
                    .ToListAsync();

            ListaTipos.Insert(0, new SelectListItem { Value = "", Text = "Todos os Tipos de Ação" });

            var viewModel = new HistoricoAcoesViewModel
            {
                Historico = listaHistoricoPaginada,
                AdminOpcoes = ListaAdmin,
                TipoAcaoOpcoes = ListaTipos,
                FiltroAdmin = filtroAdmin,
                FiltroTipoAcao = filtroAcao,
                PageNumber = pagina,
                TotalPages = (int)Math.Ceiling(totalItens / (double)itensPorPagina)
            };

            return View(viewModel);
        }

    }
}
