using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;    
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.ViewModels;

namespace ProjetoLAWBD.Controllers {
    [Authorize(Roles = "Comprador")]
    public class CheckoutController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public CheckoutController(ApplicationDbContext context, UserManager<User> userManager, IEmailSender emailSender) {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // 1. ECRA DE RESUMO (GET)
        [HttpGet]
        public async Task<IActionResult> Index(int id) {
            var userId = _userManager.GetUserId(User);

            // 1. Carregar User e Morada numa só query (Mais eficiente)
            var userComDados = await _context.Users
                .Include(u => u.Morada)
                .FirstOrDefaultAsync(u => u.Id == userId);

            // 2. Carregar Anúncio
            var anuncio = await _context.Anuncios
                .Include(a => a.Veiculo).ThenInclude(v => v.Modelo).ThenInclude(m => m.Marca)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id);

            // 3. Validação de Estado
            if (anuncio == null || anuncio.Estado != "Ativo") {
                // Se estiver reservado, verificamos se é para ESTE utilizador
                bool reservadoParaMim = await _context.Reservas
                    .AnyAsync(r => r.IdAnuncio == id && r.IdComprador == userId && r.EstadoReserva == "Aprovada");

                if (anuncio != null && anuncio.Estado == "Reservado" && reservadoParaMim) {
                    // Deixa passar, é a reserva dele
                }
                else {
                    TempData["Erro"] = "Este veículo não está disponível para compra.";
                    return RedirectToAction("Index", "Anuncios");
                }
            }

            

            // Buscar Imagem
            var imagem = await _context.Imagens
                .Where(i => i.IdVeiculo == anuncio.IdVeiculo)
                .OrderBy(i => i.OrdemExibicao)
                .FirstOrDefaultAsync();

            // Preencher ViewModel (Com campos SEPARADOS para a morada)
            var model = new CheckoutViewModel {
                AnuncioId = anuncio.IdAnuncio,
                TituloAnuncio = anuncio.Titulo,
                Preco = anuncio.Preco,
                MarcaModelo = $"{anuncio.Veiculo.Modelo.Marca.Nome} {anuncio.Veiculo.Modelo.Nome}",
                ImagemUrl = imagem?.CaminhoFicheiro ?? "~/images/placeholder-anuncio.png",

                // Dados Pessoais
                NomeFatura = userComDados.NomeCompleto, // Correção: Lemos diretamente do objeto carregado

                // Morada
                RuaENumero = userComDados.Morada?.RuaENumero,
                CodigoPostal = userComDados.Morada?.CodigoPostal,
                Cidade = userComDados.Morada?.Cidade,
                Distrito = userComDados.Morada?.Distrito


            };

            return View(model);
        }

        // 2. PROCESSAR PAGAMENTO (POST) com Lógica de Limpeza de Pedidos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessarCompra(CheckoutViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var anuncio = await _context.Anuncios
                .FirstOrDefaultAsync(a => a.IdAnuncio == model.AnuncioId);

            // Buscar Vendedor
            var vendedor = await _context.Vendedores
                .FirstOrDefaultAsync(v => v.IdUtilizador == anuncio.IdVendedor);

            if (anuncio == null || anuncio.Estado == "Arquivado" || anuncio.Estado == "Vendido" || anuncio.Estado == "Pausado")
                return RedirectToAction("Index", "Anuncios");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string moradaFinal = $"{model.RuaENumero}\n{model.CodigoPostal} {model.Cidade}\n{model.Distrito}";

                // 1. Criar o registo da Compra
                var compra = new Compra
                {
                    IdComprador = userId,
                    IdAnuncio = model.AnuncioId,
                    DataCompra = DateTime.UtcNow,
                    ValorTotal = model.Preco,
                    EstadoPagamento = "Pago",
                    NumeroFatura = $"FAT-{DateTime.Now.Year}/{new Random().Next(10000, 99999)}",
                    NomeCompradorSnapshot = model.NomeFatura,
                    MoradaCompradorSnapshot = moradaFinal,
                    NomeFaturacao = vendedor.NomeFaturacao,
                    RuaFaturacao = vendedor.RuaFaturacao,
                    CPFaturacao = vendedor.CPFaturacao,
                    CidadeFaturacao = vendedor.CidadeFaturacao,
                    NIFVendedor = vendedor.NIF,
                    IBAN = vendedor.IBAN
                };

                // 2. Atualizar estado do Anúncio
                anuncio.Estado = "Vendido";

                // ---------------------------------------------------------
                // LÓGICA DE LIMPEZA DE OUTROS PEDIDOS (IMPORTANTE)
                // ---------------------------------------------------------

                // 3. Cancelar todas as Visitas pendentes ou aprovadas para este anúncio
                var visitasParaArquivar = await _context.Visitas
                    .Where(v => v.IdAnuncio == model.AnuncioId &&
                               (v.EstadoVisita == "Pendente" || v.EstadoVisita == "Aprovada"))
                    .ToListAsync();

                foreach (var v in visitasParaArquivar)
                {
                    v.EstadoVisita = "Vendido"; // Move para o histórico do vendedor e comprador
                }

                // 4. Cancelar todas as Reservas pendentes ou aprovadas para este anúncio
                var reservasParaArquivar = await _context.Reservas
                    .Where(r => r.IdAnuncio == model.AnuncioId &&
                               (r.EstadoReserva == "Pendente" || r.EstadoReserva == "Aprovada"))
                    .ToListAsync();

                foreach (var r in reservasParaArquivar)
                {
                    r.EstadoReserva = "Vendido"; // Move para o histórico
                }

                // Guardar todas as alterações
                _context.Compras.Add(compra);
                _context.Anuncios.Update(anuncio);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                try {
                    string nomeAnuncio = $"{anuncio.Titulo}";
                    string assunto = $"Confirmação de Compra - {compra.NumeroFatura}";

                    string mensagem = $@"
                    <div style='font-family: sans-serif; color: #333;'>
                        <h1 style='color: #2c3e50;'>Obrigado pela sua compra!</h1>
                        <p>Olá <strong>{model.NomeFatura}</strong>,</p>
                        <p>O pagamento foi confirmado e o veículo é oficialmente seu.</p>
                    
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='margin-top: 0;'>Detalhes do Pedido</h3>
                            <p><strong>Veículo:</strong> {nomeAnuncio}</p>
                            <p><strong>Valor Total:</strong> {model.Preco:C2}</p>
                            <p><strong>Método Pagamento:</strong> {model.MetodoPagamento}</p>
                            <p><strong>Nº Fatura:</strong> {compra.NumeroFatura}</p>
                        </div>

                        <p>Pode consultar a fatura detalhada e o estado do pedido na sua área de cliente.</p>
                    
                        <br>
                        <a href='https://localhost:5000/Comprador/MinhasCompras' 
                           style='background-color: #000; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                           Ver Minhas Compras
                        </a>
                    </div>";

                    await _emailSender.SendEmailAsync(user.Email, assunto, mensagem);
                } catch (Exception ex) {
                    // Log do erro (opcional), mas não impedimos a venda de concluir
                    Console.WriteLine($"Erro ao enviar recibo: {ex.Message}");
                }

                return RedirectToAction("Sucesso", new { id = compra.IdCompra });
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Erro ao processar o pagamento e atualizar os pedidos.");
                return View("Index", model);
            }
        }

        // 3. PÁGINA DE AGRADECIMENTO
        public IActionResult Sucesso(int id) {
            return View(id); // Passa o ID da compra para mostrar botão "Ver Fatura"
        }
    }
}
