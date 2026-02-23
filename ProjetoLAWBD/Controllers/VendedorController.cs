using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.ViewModels;
using System.Data.Common;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProjetoLAWBD.Controllers {
    [Authorize(Roles = "Vendedor")]
    public class VendedorController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IServiceProvider _serviceProvider;

        public VendedorController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IServiceProvider serviceProvider) {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _serviceProvider = serviceProvider;
        }

        // ============================================================
        // PÁGINA: MEUS ANÚNCIOS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> MeusAnuncios(string status = "Ativos", int page = 1) {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return NotFound("Utilizador não encontrado");

            var baseQueryAnuncios = _context.Anuncios.Where(a => a.IdVendedor == userId);

            int anunciosAtivosCount = await baseQueryAnuncios.CountAsync(a => a.Estado == "Ativo" || a.Estado == "Reservado");
            int anunciosPausadosCount = await baseQueryAnuncios.CountAsync(a => a.Estado == "Pausado");
            int veiculosVendidosCount = await baseQueryAnuncios.CountAsync(a => a.Estado == "Vendido");
            int anunciosArquivadosCount = await baseQueryAnuncios.CountAsync(a => a.Estado == "Arquivado");

            int pedidosReservasPendentes = await _context.Reservas.CountAsync(r => r.Anuncio.IdVendedor == userId && r.EstadoReserva == "Pendente");
            int pedidosVisitasPendentes = await _context.Visitas.CountAsync(v => v.Anuncio.IdVendedor == userId && v.EstadoVisita == "Pendente");
            int pedidosPendentesCount = pedidosReservasPendentes + pedidosVisitasPendentes;

            IQueryable<Anuncio> queryFiltrada;
            if (status == "Pausados") queryFiltrada = baseQueryAnuncios.Where(a => a.Estado == "Pausado");
            else if (status == "Vendidos") queryFiltrada = baseQueryAnuncios.Where(a => a.Estado == "Vendido");
            else if (status == "Arquivados") queryFiltrada = baseQueryAnuncios.Where(a => a.Estado == "Arquivado");
            else queryFiltrada = baseQueryAnuncios.Where(a => a.Estado == "Ativo" || a.Estado == "Reservado");

            int pageSize = 5;
            int pageNumber = (page < 1) ? 1 : page;
            int totalCount = await queryFiltrada.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var anunciosParaMostrar = await queryFiltrada
                .Include(a => a.Veiculo).ThenInclude(v => v.Modelo).ThenInclude(m => m.Marca)
                .Include(a => a.Veiculo.Combustivel)
                .OrderByDescending(a => a.DataPublicacao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var veiculoIds = anunciosParaMostrar.Select(a => a.IdVeiculo).Distinct().ToList();
            var imagensDosVeiculos = await _context.Imagens.Where(i => veiculoIds.Contains(i.IdVeiculo)).ToListAsync();

            var viewModel = new VendedorMeusAnunciosViewModel {
                AnunciosAtivos = anunciosAtivosCount,
                AnunciosPausados = anunciosPausadosCount,
                VeiculosVendidos = veiculosVendidosCount,
                PedidosPendentes = pedidosPendentesCount,
                AnunciosArquivados = anunciosArquivadosCount,
                Anuncios = anunciosParaMostrar,
                ImagensDosVeiculos = imagensDosVeiculos,
                StatusAtual = status,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditarAnuncio(int id) {
            var userId = _userManager.GetUserId(User);
            var anuncio = await _context.Anuncios
                .Include(a => a.Veiculo)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id && a.IdVendedor == userId);

            if (anuncio == null) return NotFound();

            var imagensDoVeiculo = await _context.Imagens
                .Where(i => i.IdVeiculo == anuncio.IdVeiculo)
                .OrderBy(i => i.OrdemExibicao).ToListAsync();

            var viewModel = new VendedorMeusAnunciosEditarAnuncioViewModel {
                Anuncio = anuncio,
                Imagens = imagensDoVeiculo,
                Marcas = await _context.Marcas.ToListAsync(),
                Modelos = await _context.Modelos.ToListAsync(),
                Cores = await _context.Cores.ToListAsync(),
                Caixas = await _context.Caixas.ToListAsync(),
                Combustiveis = await _context.Combustiveis.ToListAsync(),
                Categorias = await _context.Categorias.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAnuncio(VendedorMeusAnunciosEditarAnuncioViewModel viewModel) {
            var userId = _userManager.GetUserId(User);

            ModelState.Remove("Anuncio.Vendedor"); ModelState.Remove("Anuncio.Veiculo"); ModelState.Remove("Anuncio.IdVendedor");
            ModelState.Remove("Anuncio.DataPublicacao"); ModelState.Remove("Anuncio.Estado"); ModelState.Remove("Veiculo");
            ModelState.Remove("Cores"); ModelState.Remove("Caixas"); ModelState.Remove("Marcas"); ModelState.Remove("Imagens");
            ModelState.Remove("Modelos"); ModelState.Remove("Categorias"); ModelState.Remove("Combustiveis");

            if (!ModelState.IsValid) {
                var veiculoAssociado = await _context.Veiculos.Include(v => v.Modelo).ThenInclude(m => m.Marca)
                    .FirstOrDefaultAsync(v => v.IdVeiculo == viewModel.Anuncio.IdVeiculo);

                if (veiculoAssociado == null) {
                    var anuncioTemp = await _context.Anuncios.Include(a => a.Veiculo).ThenInclude(v => v.Modelo).ThenInclude(m => m.Marca)
                        .FirstOrDefaultAsync(a => a.IdAnuncio == viewModel.Anuncio.IdAnuncio);
                    veiculoAssociado = anuncioTemp?.Veiculo;
                }

                if (veiculoAssociado != null) viewModel.Anuncio.Veiculo = veiculoAssociado;
                return View(viewModel);
            }

            var anuncioDb = await _context.Anuncios.FirstOrDefaultAsync(a => a.IdAnuncio == viewModel.Anuncio.IdAnuncio && a.IdVendedor == userId);
            if (anuncioDb == null) return NotFound();

            anuncioDb.Titulo = viewModel.Anuncio.Titulo;
            anuncioDb.Descricao = viewModel.Anuncio.Descricao;
            anuncioDb.Preco = viewModel.Anuncio.Preco;
            anuncioDb.LocalizacaoCidade = viewModel.Anuncio.LocalizacaoCidade;

            try {
                _context.Update(anuncioDb);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Anúncio atualizado com sucesso!";
                return RedirectToAction(nameof(MeusAnuncios));
            } catch (Exception) {
                ModelState.AddModelError("", "Ocorreu um erro ao salvar as alterações.");
                var veiculoRecarga = await _context.Veiculos.Include(v => v.Modelo).ThenInclude(m => m.Marca)
                     .FirstOrDefaultAsync(v => v.IdVeiculo == viewModel.Anuncio.IdVeiculo);
                viewModel.Anuncio.Veiculo = veiculoRecarga;
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PausarAnuncio(int id) {
            var userId = _userManager.GetUserId(User);
            var anuncio = await _context.Anuncios.FirstOrDefaultAsync(a => a.IdAnuncio == id && a.IdVendedor == userId);

            if (anuncio == null) return NotFound();

            anuncio.Estado = "Pausado";
            _context.Update(anuncio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MeusAnuncios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtivarAnuncio(int id, string returnUrl) {
            var userId = _userManager.GetUserId(User);
            var anuncio = await _context.Anuncios.FirstOrDefaultAsync(a => a.IdAnuncio == id && a.IdVendedor == userId);

            if (anuncio == null) return NotFound();

            anuncio.Estado = "Ativo";
            _context.Update(anuncio);
            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            else return RedirectToAction(nameof(MeusAnuncios), new { status = "Pausados" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArquivarAnuncio(int id) {
            var userId = _userManager.GetUserId(User);
            var anuncio = await _context.Anuncios.FirstOrDefaultAsync(a => a.IdAnuncio == id && a.IdVendedor == userId);

            if (anuncio == null) return NotFound();

            anuncio.Estado = "Arquivado";
            _context.Update(anuncio);
            await _context.SaveChangesAsync();

            string referer = Request.Headers["Referer"].ToString();
            return Redirect(referer ?? Url.Action(nameof(MeusAnuncios)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IrParaEditarVeiculo(VendedorEditarVeiculoViewModel viewModel) {
            return View();
        }

        // ============================================================
        // PÁGINA: GERIR PEDIDOS (COM LÓGICA MERGED + TÍTULO DO ANÚNCIO)
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> GerirPedidos(string status = "Pendentes") {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return NotFound("Utilizador não encontrado");

            var viewModel = await ObterGerirPedidosViewModel(userId, status);
            return View(viewModel);
        }

        // --- O MÉTODO GENÉRICO DO TEU COLEGA (AGORA COM A TUA LÓGICA DE NOTIFICAÇÃO) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessarPedido(int id, string tipo, string acao, string statusAtual) {
            var userId = _userManager.GetUserId(User);

            // Variáveis para configurar a Notificação
            bool enviarEmail = false;
            string idComprador = "";
            string emailComprador = "";
            string emailAssunto = "";
            string emailCorpo = "";

            if (tipo == "Visita") {
                // Incluímos o Anúncio (para ter o Título) e o Comprador (para ter o Email)
                var visita = await _context.Visitas
                    .Include(v => v.Anuncio) // Importante para o v.Anuncio.Titulo
                    .Include(v => v.Comprador.user)
                    .FirstOrDefaultAsync(v => v.IdVisita == id && v.Anuncio.IdVendedor == userId);

                if (visita != null) {
                    // === AQUI: Usamos o TÍTULO DO ANÚNCIO, como tinhas originalmente ===
                    string nomeAnuncio = visita.Anuncio.Titulo;

                    idComprador = visita.IdComprador;
                    emailComprador = visita.Comprador.user.Email;

                    if (acao == "Aprovar") {
                        visita.EstadoVisita = "Aprovada";
                        enviarEmail = true;
                        emailAssunto = "Visita Confirmada ✅";
                        emailCorpo = $@"<h1>Visita Agendada!</h1>
                                        <p>A sua visita para ver o <strong>{nomeAnuncio}</strong> foi confirmada.</p>
                                        <p><strong>Data:</strong> {visita.DataVisita:dd/MM/yyyy às HH:mm}</p>
                                        <p>O vendedor estará à sua espera no local.</p>";
                    }
                    else if (acao == "Rejeitar") {
                        visita.EstadoVisita = "Rejeitada";
                        enviarEmail = true;
                        emailAssunto = "Pedido de Visita Recusado";
                        emailCorpo = $@"<p>O vendedor não pôde aceitar o agendamento da visita para o <strong>{nomeAnuncio}</strong> nesta data.</p>
                                        <p>Sugerimos que tente entrar em contacto para marcar noutro horário.</p>";
                    }
                    else if (acao == "Cancelar") {
                        visita.EstadoVisita = "Cancelada";
                    }
                    _context.Update(visita);
                }
            }
            else if (tipo == "Reserva") {
                var reserva = await _context.Reservas
                    .Include(r => r.Anuncio) // Importante para o r.Anuncio.Titulo
                    .Include(r => r.Comprador.user)
                    .FirstOrDefaultAsync(r => r.IdReserva == id && r.Anuncio.IdVendedor == userId);

                if (reserva != null) {
                    // === AQUI: Usamos o TÍTULO DO ANÚNCIO, como tinhas originalmente ===
                    string nomeAnuncio = reserva.Anuncio.Titulo;

                    idComprador = reserva.IdComprador;
                    emailComprador = reserva.Comprador.user.Email;

                    if (acao == "Aprovar") {
                        reserva.EstadoReserva = "Aprovada";
                        reserva.Anuncio.Estado = "Reservado";
                        reserva.PrazoExpiracao = DateTime.Now.AddDays(7);

                        enviarEmail = true;
                        emailAssunto = "Reserva Aprovada! 🚗";
                        emailCorpo = $@"<h1>Boas notícias!</h1>
                                        <p>O vendedor aceitou a sua reserva para o <strong>{nomeAnuncio}</strong>.</p>
                                        <p>Tem até {reserva.PrazoExpiracao:dd/MM/yyyy HH:mm} para concluir o negócio.</p>
                                        <p><a href='https://localhost:5000/Comprador/MinhasCompras'>Ver nas Minhas Compras</a></p>";
                    }
                    else if (acao == "Rejeitar") {
                        reserva.EstadoReserva = "Rejeitada";
                        enviarEmail = true;
                        emailAssunto = "Atualização sobre a sua reserva";
                        emailCorpo = $@"<p>Infelizmente, o vendedor não aceitou o pedido de reserva para o <strong>{nomeAnuncio}</strong>.</p>
                                        <p>Continue a procurar no AutoMarket, temos muitos outros veículos!</p>";
                    }
                    else if (acao == "Cancelar") {
                        reserva.EstadoReserva = "Cancelada";
                        reserva.Anuncio.Estado = "Ativo";
                    }
                    _context.Update(reserva);
                }
            }

            // 1. Gravar alterações
            await _context.SaveChangesAsync();

            // 2. Enviar Notificação (Se aplicável)
            if (enviarEmail && !string.IsNullOrEmpty(emailComprador)) {
                await NotificarAlteracaoEstado(idComprador, emailComprador, emailAssunto, emailCorpo);
            }

            // 3. Devolver a Partial View atualizada (AJAX)
            var viewModel = await ObterGerirPedidosViewModel(userId, statusAtual);
            return PartialView("_ListaPedidosPartial", viewModel);
        }

        private async Task<VendedorGerirPedidosViewModel> ObterGerirPedidosViewModel(string userId, string status) {
            var baseQueryVisitas = _context.Visitas
                .Include(v => v.Anuncio).ThenInclude(a => a.Veiculo).ThenInclude(m => m.Modelo)
                .Include(v => v.Comprador.user)
                .Where(v => v.Anuncio.IdVendedor == userId);

            var baseQueryReservas = _context.Reservas
                .Include(r => r.Anuncio).ThenInclude(a => a.Veiculo).ThenInclude(m => m.Modelo)
                .Include(r => r.Comprador.user)
                .Where(r => r.Anuncio.IdVendedor == userId);

            int pendentes = await baseQueryVisitas.CountAsync(v => v.EstadoVisita == "Pendente") +
                            await baseQueryReservas.CountAsync(r => r.EstadoReserva == "Pendente");
            int aprovados = await baseQueryVisitas.CountAsync(v => v.EstadoVisita == "Aprovada") +
                            await baseQueryReservas.CountAsync(r => r.EstadoReserva == "Aprovada");
            int historico = await baseQueryVisitas.CountAsync(v => v.EstadoVisita != "Pendente" && v.EstadoVisita != "Aprovada") +
                            await baseQueryReservas.CountAsync(r => r.EstadoReserva != "Pendente" && r.EstadoReserva != "Aprovada");

            IQueryable<Visita> visitas;
            IQueryable<Reserva> reservas;

            if (status == "Aprovados") {
                visitas = baseQueryVisitas.Where(v => v.EstadoVisita == "Aprovada");
                reservas = baseQueryReservas.Where(r => r.EstadoReserva == "Aprovada");
            }
            else if (status == "Historico") {
                visitas = baseQueryVisitas.Where(v => v.EstadoVisita != "Pendente" && v.EstadoVisita != "Aprovada");
                reservas = baseQueryReservas.Where(r => r.EstadoReserva != "Pendente" && r.EstadoReserva != "Aprovada");
            }
            else {
                visitas = baseQueryVisitas.Where(v => v.EstadoVisita == "Pendente");
                reservas = baseQueryReservas.Where(r => r.EstadoReserva == "Pendente");
            }

            return new VendedorGerirPedidosViewModel {
                Pendentes = pendentes,
                Aprovados = aprovados,
                Historico = historico,
                PedidosVisita = await visitas.ToListAsync(),
                PedidosReserva = await reservas.ToListAsync(),
                StatusAtual = status
            };
        }

        // ============================================================
        // PÁGINA: MEUS VEÍCULOS (GET / ADD / EDIT / REMOVE)
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> MeusVeiculos(int page = 1) {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return NotFound("Utilizador não encontrado");

            int pageSize = 5;
            int pageNumber = (page < 1) ? 1 : page;

            var queryBase = _context.Veiculos.Include(v => v.Modelo.Marca).Where(v => v.IdVendedor == userId && v.Arquivado == false);

            int totalCount = await queryBase.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var veiculos = await queryBase.OrderByDescending(v => v.DataMatricula)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var veiculosIds = veiculos.Select(v => v.IdVeiculo).ToList();
            var imagensDosVeiculos = await _context.Imagens.Where(i => veiculosIds.Contains(i.IdVeiculo)).ToListAsync();

            var anunciosRelevantes = await _context.Anuncios
                .Where(a => a.IdVendedor == userId && (a.Estado == "Ativo" || a.Estado == "Reservado" || a.Estado == "Pausado" || a.Estado == "Vendido"))
                .Select(a => new { a.IdVeiculo, a.IdAnuncio, a.Estado }).ToListAsync();

            var viewModelList = new List<VeiculoComStatus>();
            foreach (var veiculo in veiculos) {
                var anuncio = anunciosRelevantes.FirstOrDefault(a => a.IdVeiculo == veiculo.IdVeiculo);
                string status = "Não Listado";
                int? anuncioId = null;

                if (anuncio != null) {
                    anuncioId = anuncio.IdAnuncio;
                    if (anuncio.Estado == "Vendido") status = "Vendido";
                    else if (anuncio.Estado == "Pausado") status = "Pausado";
                    else if (anuncio.Estado == "Reservado") status = "Reservado";
                    else status = "À Venda";
                }
                viewModelList.Add(new VeiculoComStatus { Veiculo = veiculo, StatusDoAnuncio = status, AnuncioId = anuncioId });
            }

            var pageViewModel = new VendedorMeusVeiculosViewModel {
                Veiculos = viewModelList,
                ImagensDoVeiculos = imagensDosVeiculos,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };
            return View(pageViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AdicionarVeiculo() {
            var viewModel = new VendedorAdicionarVeiculoViewModel {
                Veiculo = new Veiculo(),
                Marcas = await _context.Marcas.ToListAsync(),
                Modelos = await _context.Modelos.ToListAsync(),
                Caixas = await _context.Caixas.ToListAsync(),
                Combustiveis = await _context.Combustiveis.ToListAsync(),
                Cores = await _context.Cores.ToListAsync(),
                Categorias = await _context.Categorias.ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarVeiculo(VendedorAdicionarVeiculoViewModel viewModel) {
            var userId = _userManager.GetUserId(User);
            viewModel.Veiculo.IdVendedor = userId;

            ModelState.Remove("Marcas"); ModelState.Remove("Modelos"); ModelState.Remove("Cores"); ModelState.Remove("Caixas");
            ModelState.Remove("Combustiveis"); ModelState.Remove("Categorias"); ModelState.Remove("FicheirosImagem");
            ModelState.Remove("Veiculo.IdVeiculo"); ModelState.Remove("Veiculo.IdVendedor"); ModelState.Remove("Veiculo.Vendedor");
            ModelState.Remove("Veiculo.Modelo"); ModelState.Remove("Veiculo.Cor"); ModelState.Remove("Veiculo.Caixa");
            ModelState.Remove("Veiculo.Combustivel"); ModelState.Remove("Veiculo.Categoria");

            if (ModelState.IsValid) {
                using (var transaction = await _context.Database.BeginTransactionAsync()) {
                    try {
                        _context.Veiculos.Add(viewModel.Veiculo);
                        await _context.SaveChangesAsync();

                        if (viewModel.FicheirosImagem != null && viewModel.FicheirosImagem.Count > 0) {
                            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/veiculos");
                            Directory.CreateDirectory(uploadsFolder);
                            int ordem = 1;

                            foreach (var ficheiroJson in viewModel.FicheirosImagem) {
                                var jsonDoc = JsonDocument.Parse(ficheiroJson);
                                var data = jsonDoc.RootElement.GetProperty("data").GetString();
                                var nome = jsonDoc.RootElement.GetProperty("name").GetString();
                                byte[] bytes = Convert.FromBase64String(data);

                                string nomeFicheiroUnico = Guid.NewGuid().ToString() + "_" + nome;
                                string caminhoCompleto = Path.Combine(uploadsFolder, nomeFicheiroUnico);
                                await System.IO.File.WriteAllBytesAsync(caminhoCompleto, bytes);

                                var novaImagem = new Imagem {
                                    IdVeiculo = viewModel.Veiculo.IdVeiculo,
                                    CaminhoFicheiro = $"/uploads/veiculos/{nomeFicheiroUnico}",
                                    IsCapa = (ordem == 1),
                                    OrdemExibicao = ordem
                                };
                                await _context.Imagens.AddAsync(novaImagem);
                                ordem++;
                            }
                            await _context.SaveChangesAsync();
                        }
                        await transaction.CommitAsync();
                        return RedirectToAction(nameof(MeusVeiculos));
                    } catch (Exception) {
                        TempData["ErrorMessage"] = "Não foi possível adicionar veículo.";
                        return RedirectToAction(nameof(MeusVeiculos));
                    }
                }
            }

            viewModel.Marcas = await _context.Marcas.ToListAsync();
            viewModel.Modelos = await _context.Modelos.ToListAsync();
            viewModel.Cores = await _context.Cores.ToListAsync();
            viewModel.Caixas = await _context.Caixas.ToListAsync();
            viewModel.Combustiveis = await _context.Combustiveis.ToListAsync();
            viewModel.Categorias = await _context.Categorias.ToListAsync();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditarVeiculo(int id) {
            var userId = _userManager.GetUserId(User);
            var veiculo = await _context.Veiculos.Include(v => v.Modelo).FirstOrDefaultAsync(v => v.IdVeiculo == id && v.IdVendedor == userId);
            if (veiculo == null) return NotFound("Veículo não encontrado ou não lhe pertence.");

            var viewModel = new VendedorEditarVeiculoViewModel {
                Veiculo = veiculo,
                ImagensExistentes = await _context.Imagens.Where(i => i.IdVeiculo == id).OrderBy(i => i.OrdemExibicao).ToListAsync(),
                Marcas = await _context.Marcas.ToListAsync(),
                Modelos = await _context.Modelos.ToListAsync(),
                Cores = await _context.Cores.ToListAsync(),
                Caixas = await _context.Caixas.ToListAsync(),
                Combustiveis = await _context.Combustiveis.ToListAsync(),
                Categorias = await _context.Categorias.ToListAsync()
            };

            bool temAnuncioAtivo = await _context.Anuncios.AnyAsync(a => a.IdVeiculo == id && (a.Estado == "Ativo" || a.Estado == "Reservado"));
            ViewBag.TemAnuncioAtivo = temAnuncioAtivo;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarVeiculo(VendedorEditarVeiculoViewModel viewModel) {
            var userId = _userManager.GetUserId(User);
            var veiculoDb = await _context.Veiculos.FirstOrDefaultAsync(v => v.IdVeiculo == viewModel.Veiculo.IdVeiculo && v.IdVendedor == userId);
            if (veiculoDb == null) return NotFound();

            ModelState.Remove("Marcas"); ModelState.Remove("Modelos"); ModelState.Remove("Cores");
            ModelState.Remove("Caixas"); ModelState.Remove("Combustiveis"); ModelState.Remove("Categorias");
            ModelState.Remove("idsImagensParaRemover"); ModelState.Remove("Veiculo.IdVendedor");
            ModelState.Remove("FicheirosImagem"); ModelState.Remove("ImagensExistentes");
            ModelState.Remove("Veiculo.Vendedor"); ModelState.Remove("Veiculo.Modelo");
            ModelState.Remove("Veiculo.Cor"); ModelState.Remove("Veiculo.Caixa");
            ModelState.Remove("Veiculo.Combustivel"); ModelState.Remove("Veiculo.Categoria");

            if (ModelState.IsValid) {
                // Atualizar dados
                veiculoDb.IdModelo = viewModel.Veiculo.IdModelo;
                veiculoDb.IdCor = viewModel.Veiculo.IdCor;
                veiculoDb.IdCaixa = viewModel.Veiculo.IdCaixa;
                veiculoDb.IdCombustivel = viewModel.Veiculo.IdCombustivel;
                veiculoDb.IdCategoria = viewModel.Veiculo.IdCategoria;
                veiculoDb.PotenciaCV = viewModel.Veiculo.PotenciaCV;
                veiculoDb.CilindradaCC = viewModel.Veiculo.CilindradaCC;
                veiculoDb.NumLugares = viewModel.Veiculo.NumLugares;
                veiculoDb.NumPortas = viewModel.Veiculo.NumPortas;
                veiculoDb.KmTotal = viewModel.Veiculo.KmTotal;
                veiculoDb.DataMatricula = viewModel.Veiculo.DataMatricula;
                _context.Update(veiculoDb);

                // Remover imagens antigas
                if (!string.IsNullOrEmpty(viewModel.idsImagensParaRemover)) {
                    var ids = viewModel.idsImagensParaRemover.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    var imgsParaRemover = await _context.Imagens.Where(i => ids.Contains(i.IdImagem)).ToListAsync();
                    foreach (var img in imgsParaRemover) {
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, img.CaminhoFicheiro.TrimStart('/'));
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                        _context.Imagens.Remove(img);
                    }
                }

                // Novas imagens
                if (viewModel.FicheirosImagem != null && viewModel.FicheirosImagem.Count > 0) {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/veiculos");
                    Directory.CreateDirectory(uploadsFolder);
                    int ordemAtual = 1;

                    foreach (var json in viewModel.FicheirosImagem) {
                        using var doc = JsonDocument.Parse(json);
                        var root = doc.RootElement;
                        int? idImagemAntiga = null;
                        if (root.TryGetProperty("metadata", out var metaProp) && metaProp.TryGetProperty("idDataBase", out var idStr)) {
                            if (int.TryParse(idStr.GetString(), out int idParsed)) idImagemAntiga = idParsed;
                        }

                        if (idImagemAntiga.HasValue) {
                            var imgExistente = await _context.Imagens.FindAsync(idImagemAntiga.Value);
                            if (imgExistente != null) {
                                imgExistente.OrdemExibicao = ordemAtual;
                                imgExistente.IsCapa = (ordemAtual == 1);
                                _context.Update(imgExistente);
                            }
                        }
                        else if (root.TryGetProperty("data", out var dataProp)) {
                            var bytes = Convert.FromBase64String(dataProp.GetString());
                            var nome = root.GetProperty("name").GetString();
                            var nomeUnico = Guid.NewGuid() + "_" + nome;
                            await System.IO.File.WriteAllBytesAsync(Path.Combine(uploadsFolder, nomeUnico), bytes);

                            var novaImagem = new Imagem {
                                IdVeiculo = veiculoDb.IdVeiculo,
                                CaminhoFicheiro = $"/uploads/veiculos/{nomeUnico}",
                                IsCapa = (ordemAtual == 1),
                                OrdemExibicao = ordemAtual
                            };
                            _context.Imagens.Add(novaImagem);
                        }
                        ordemAtual++;
                    }
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Veículo atualizado!";
                return RedirectToAction(nameof(MeusVeiculos));
            }
            await RecarregarListasViewModel(viewModel);
            return View(viewModel);
        }

        private async Task RecarregarListasViewModel(VendedorEditarVeiculoViewModel vm) {
            vm.Marcas = await _context.Marcas.ToListAsync();
            vm.Modelos = await _context.Modelos.ToListAsync();
            vm.Cores = await _context.Cores.ToListAsync();
            vm.Caixas = await _context.Caixas.ToListAsync();
            vm.Combustiveis = await _context.Combustiveis.ToListAsync();
            vm.Categorias = await _context.Categorias.ToListAsync();
            vm.ImagensExistentes = await _context.Imagens.Where(i => i.IdVeiculo == vm.Veiculo.IdVeiculo).ToListAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArquivarVeiculo(int id, int pageAtual) {
            var userId = _userManager.GetUserId(User);

            // 1. Verificar se o veículo existe e pertence ao utilizador
            var veiculo = await _context.Veiculos
                .FirstOrDefaultAsync(v => v.IdVeiculo == id && v.IdVendedor == userId);

            if (veiculo == null) return NotFound();

            // 2. Segurança: Impedir arquivar se houver anúncios ativos/pendentes
            bool temAnuncioAtivo = await _context.Anuncios
                .AnyAsync(a => a.IdVeiculo == id && (a.Estado == "Ativo" || a.Estado == "Reservado" || a.Estado == "Pausado"));

            if (temAnuncioAtivo) {

                // Usamos ViewBag para enviar o erro diretamente para a Partial View que vai ser renderizada agora.
                ViewBag.ErrorMessage = "Não é possível arquivar este veículo porque existem anúncios ativos, reservados ou pausados associados a ele.";

                var vmErro = await ObterMeusVeiculosViewModel(userId, pageAtual);
                return PartialView("_MeusVeiculosListaPartial", vmErro);
            }

            // 3. Limpeza de Imagens (Física + Base de Dados)
            var imagens = await _context.Imagens.Where(i => i.IdVeiculo == id).ToListAsync();

            if (imagens.Any()) {
                string webRootPath = _webHostEnvironment.WebRootPath;

                foreach (var img in imagens) {
                    // Converter caminho relativo (/uploads/...) em absoluto (C:\Inetpub\...)
                    var caminhoFicheiro = Path.Combine(webRootPath, img.CaminhoFicheiro.TrimStart('/', '\\'));

                    if (System.IO.File.Exists(caminhoFicheiro)) {
                        try {
                            System.IO.File.Delete(caminhoFicheiro);
                        } catch (Exception ex) {
                            // Se o ficheiro estiver bloqueado pelo Windows, fazemos log e continuamos
                            Console.WriteLine($"Erro ao apagar ficheiro físico: {ex.Message}");
                        }
                    }
                }

                // Remover os registos da tabela Imagens para não ficarem links quebrados
                _context.Imagens.RemoveRange(imagens);
            }

            // 4. Soft Delete: Marcar como Arquivado
            veiculo.Arquivado = true;
            _context.Update(veiculo);

            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Veículo arquivado com sucesso.";

            // 5. Recarregar a lista atualizada para o AJAX substituir o HTML
            var viewModel = await ObterMeusVeiculosViewModel(userId, pageAtual);

            return PartialView("_MeusVeiculosListaPartial", viewModel);
        }

        // ============================================================
        // PÁGINA: CRIAR E CONFIRMAR ANÚNCIO
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> CriarAnuncio(int veiculoId) {
            var userId = _userManager.GetUserId(User);
            var veiculo = await _context.Veiculos.Include(v => v.Modelo.Marca)
                .FirstOrDefaultAsync(v => v.IdVeiculo == veiculoId && v.IdVendedor == userId);

            if (veiculo == null) return NotFound("Veículo não encontrado ou não lhe pertence.");

            var viewModel = new CriarAnuncioViewModel {
                Veiculo = veiculo,
                Anuncio = new Anuncio() { IdVeiculo = veiculoId }
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarAnuncio(CriarAnuncioViewModel viewModel) {
            var userId = _userManager.GetUserId(User);
            viewModel.Anuncio.IdVendedor = userId;
            viewModel.Anuncio.DataPublicacao = DateTime.UtcNow;
            viewModel.Anuncio.Estado = "Ativo";

            ModelState.Remove("Imagens"); ModelState.Remove("Veiculo"); ModelState.Remove("Anuncio.Vendedor");
            ModelState.Remove("Anuncio.Veiculo"); ModelState.Remove("Anuncio.IdVendedor");
            ModelState.Remove("Anuncio.Estado"); ModelState.Remove("Anuncio.DataPublicacao");

            if (ModelState.IsValid) {
                TempData["AnuncioPendente"] = JsonSerializer.Serialize(viewModel.Anuncio);
                return RedirectToAction(nameof(ConfirmarAnuncio));
            }

            var veiculo = await _context.Veiculos.Include(v => v.Modelo.Marca)
                .FirstOrDefaultAsync(v => v.IdVeiculo == viewModel.Anuncio.IdVeiculo);
            viewModel.Veiculo = veiculo;
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarAnuncio() {
            if (TempData["AnuncioPendente"] is not string anuncioJson) return RedirectToAction(nameof(MeusVeiculos));
            TempData.Keep("AnuncioPendente");

            var anuncio = JsonSerializer.Deserialize<Anuncio>(anuncioJson);
            var veiculo = await _context.Veiculos.Include(v => v.Modelo.Marca).Include(v => v.Cor)
                .Include(v => v.Combustivel).Include(v => v.Caixa).Include(v => v.Categoria)
                .FirstOrDefaultAsync(v => v.IdVeiculo == anuncio.IdVeiculo);

            if (veiculo == null) return NotFound("Veículo não encontrado.");

            var imagens = await _context.Imagens.Where(i => i.IdVeiculo == anuncio.IdVeiculo).OrderBy(i => i.OrdemExibicao).ToListAsync();
            var viewModel = new CriarAnuncioViewModel { Anuncio = anuncio, Veiculo = veiculo, Imagens = imagens };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarAnuncio(CriarAnuncioViewModel viewModel) {
            if (TempData["AnuncioPendente"] is not string anuncioJson) return RedirectToAction(nameof(MeusVeiculos));

            var anuncioParaGuardar = JsonSerializer.Deserialize<Anuncio>(anuncioJson);

            try {
                _context.Anuncios.Add(anuncioParaGuardar);
                await _context.SaveChangesAsync();

                // -------------------------------------------------------------------------
                // GATILHO: NOVOS ANÚNCIOS (BACKGROUND TASK)
                // -------------------------------------------------------------------------
                var idAnuncioNovo = anuncioParaGuardar.IdAnuncio;
                var idVeiculo = anuncioParaGuardar.IdVeiculo;

                _ = Task.Run(async () => {
                    using (var scope = _serviceProvider.CreateScope()) {
                        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                        var dadosCarro = await ctx.Veiculos.Include(v => v.Modelo.Marca).FirstOrDefaultAsync(v => v.IdVeiculo == idVeiculo);

                        if (dadosCarro != null) {
                            // 1. Quem gosta da marca?
                            var idsInteressados = await ctx.MarcasFavoritasCompradores
                                .Where(mf => mf.IdMarca == dadosCarro.Modelo.IdMarca)
                                .Select(mf => mf.IdComprador).ToListAsync();

                            // 2. Quem recusou explicitamente?
                            var idsRecusaram = await ctx.ConfNotificacoes
                                .Where(cn => idsInteressados.Contains(cn.IdUtilizador) && cn.AnunciosMarcasFavoritas == false)
                                .Select(cn => cn.IdUtilizador).ToListAsync();

                            // 3. Lista final (Opt-out logic)
                            var idsFinais = idsInteressados.Except(idsRecusaram).ToList();

                            var emailsParaNotificar = await ctx.Users
                                .Where(u => idsFinais.Contains(u.Id))
                                .Select(u => u.Email).Distinct().ToListAsync();

                            foreach (var email in emailsParaNotificar) {
                                await emailSender.SendEmailAsync(email,
                                    $"Novo {dadosCarro.Modelo.Marca.Nome} disponível!",
                                    $"<h1>Novidade AutoMarket</h1><p>Acabou de chegar um <strong>{dadosCarro.Modelo.Marca.Nome}</strong>.</p><p>Preço: {anuncioParaGuardar.Preco:C0}</p><p><a href='https://localhost:5000/Anuncios/Detalhes/{idAnuncioNovo}'>Ver Anúncio</a></p>");
                            }
                        }
                    }
                });

                TempData.Remove("AnuncioPendente");
                return RedirectToAction(nameof(SucessoAnuncio), new { id = anuncioParaGuardar.IdAnuncio });
            } catch (Exception) {
                TempData["ErrorMessage"] = "Não foi possível publicar o anúncio.";
                return RedirectToAction(nameof(MeusVeiculos));
            }
        }

        [HttpGet]
        public async Task<IActionResult> SucessoAnuncio(int id) {
            var userId = _userManager.GetUserId(User);
            var anuncio = await _context.Anuncios.FirstOrDefaultAsync(a => a.IdAnuncio == id && a.IdVendedor == userId);
            if (anuncio == null) return NotFound();
            return View(anuncio);
        }

        [HttpGet]
        public IActionResult AguardandoAprovacao() {
            // Opcional: Se já estiver aprovado, chuta para a Dashboard
            // Mas o Filtro já deve tratar disto, por segurança deixamos simples.
            return View();
        }
        // ============================================================
        // PÁGINA: DADOS FATURAÇÃO
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> DadosFaturacao() {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return NotFound("Utilizador não encontrado");

            var vendedor = await _context.Vendedores.FindAsync(userId);
            if (vendedor == null) return NotFound("Dados do vendedor não foram encontrados");

            return View(vendedor);
        }

        

        // Atualiza o POST para redirecionar para a página de espera
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DadosFaturacao")]
        public async Task<IActionResult> DadosFaturacaoPost() {
            var userId = _userManager.GetUserId(User);
            var vendedorParaAtualizar = await _context.Vendedores.FindAsync(userId);

            if (vendedorParaAtualizar == null) return NotFound();

            try {
                if (await TryUpdateModelAsync(vendedorParaAtualizar, "",
                        v => v.NIF, v => v.NomeFaturacao, v => v.RuaFaturacao,
                        v => v.CPFaturacao, v => v.CidadeFaturacao, v => v.IBAN, v => v.TipoVendedor)) {

                    await _context.SaveChangesAsync();

                    // Em vez de voltar ao formulário, enviamos para a página de espera
                    TempData["SuccessMessage"] = "Dados submetidos! Aguarde a aprovação.";
                    return RedirectToAction(nameof(AguardandoAprovacao));
                }
            } catch (DbUpdateException) {
                ModelState.AddModelError("", "Não foi possivel guardar as alterações.");
            }
            return View(vendedorParaAtualizar);
        }

        // ============================================================
        // MÉTODO AUXILIAR PARA NOTIFICAÇÕES (OPT-OUT LOGIC)
        // ============================================================
        private async Task NotificarAlteracaoEstado(string idComprador, string emailComprador, string assunto, string mensagem) {
            try {
                var config = await _context.ConfNotificacoes.FindAsync(idComprador);
                // Se não existe config (null), assume TRUE. Só não envia se for explicitamente FALSE.
                bool enviar = config == null || config.AlteracoesReservasVisitas == true;

                if (enviar) {
                    await _emailSender.SendEmailAsync(emailComprador, assunto, mensagem);
                }
            } catch (Exception ex) {
                Console.WriteLine($"Erro ao notificar comprador: {ex.Message}");
            }
        }

        // Coloca isto dentro da classe VendedorController, lá no fundo

        private async Task<VendedorMeusVeiculosViewModel> ObterMeusVeiculosViewModel(string userId, int page) {
            int pageSize = 5; // Veículos por página

            // 1. Query Base: Veículos do vendedor que NÃO estão arquivados (Soft Delete)
            var queryBase = _context.Veiculos
                .Include(v => v.Modelo.Marca)
                .Where(v => v.IdVendedor == userId && v.Arquivado == false); // <--- O tal filtro importante

            // 2. Calcular Paginação
            int totalCount = await queryBase.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Proteção para não pedir uma página que não existe (ex: página 0 ou página 10 quando só há 2)
            int pageNumber = page;
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages && totalPages > 0) pageNumber = totalPages;

            // 3. Obter os Veículos da Página Atual
            var veiculos = await queryBase
                .OrderByDescending(v => v.DataMatricula) // Ordenar por mais recente
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 4. Obter as Imagens (apenas para os veículos desta página)
            var veiculosIds = veiculos.Select(v => v.IdVeiculo).ToList();
            var imagens = await _context.Imagens
                .Where(i => veiculosIds.Contains(i.IdVeiculo))
                .ToListAsync();

            // 5. Verificar Estados dos Anúncios
            // Precisamos de saber se o carro já está anunciado, vendido, etc.
            var anuncios = await _context.Anuncios
                .Where(a => a.IdVendedor == userId && veiculosIds.Contains(a.IdVeiculo))
                .Select(a => new { a.IdVeiculo, a.IdAnuncio, a.Estado })
                .ToListAsync();

            // 6. Construir a Lista Final com Status
            var viewModelList = new List<VeiculoComStatus>();

            foreach (var v in veiculos) {
                // Tenta encontrar um anúncio para este carro
                // Damos prioridade a anúncios que não estejam "Arquivados" se houver múltiplos,
                // mas aqui assumimos 1 anúncio ativo por carro.
                var anuncio = anuncios.FirstOrDefault(a => a.IdVeiculo == v.IdVeiculo && a.Estado != "Arquivado");

                // Se não encontrar ativo, procura se há um "Vendido" (para histórico)
                if (anuncio == null) {
                    anuncio = anuncios.FirstOrDefault(a => a.IdVeiculo == v.IdVeiculo && a.Estado == "Vendido");
                }

                string status = "Não Listado";
                int? anuncioId = null;

                if (anuncio != null) {
                    anuncioId = anuncio.IdAnuncio;

                    // Traduzir o estado da BD para o texto da View
                    status = anuncio.Estado switch {
                        "Vendido" => "Vendido",
                        "Pausado" => "Pausado",
                        "Reservado" => "Reservado",
                        "Ativo" => "À Venda",
                        _ => "Não Listado"
                    };
                }

                viewModelList.Add(new VeiculoComStatus {
                    Veiculo = v,
                    StatusDoAnuncio = status,
                    AnuncioId = anuncioId
                });
            }

            // 7. Retornar o ViewModel completo
            return new VendedorMeusVeiculosViewModel {
                Veiculos = viewModelList,
                ImagensDoVeiculos = imagens,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };
        }
    }
}