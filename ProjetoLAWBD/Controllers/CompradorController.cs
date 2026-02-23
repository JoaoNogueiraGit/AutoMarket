using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.ViewModels;

namespace ProjetoLAWBD.Controllers
{
    [Authorize(Roles = "Comprador")]
    public class CompradorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CompradorController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MarcasFavoritas()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) {
                return NotFound();
            }

            var viewmodel = new CompradorMarcasFavoritasViewModel
            {
                TodasMarcas = await _context.Marcas.Select(m => new SelectListItem { Value = m.IdMarca.ToString(), Text = m.Nome }).ToListAsync(),
                MinhasMarcas = await _context.MarcasFavoritasCompradores.Include(mf => mf.Marca).Where(mf => mf.IdComprador == user.Id).ToListAsync()
            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarMarcaFavorita(int idMarca)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!await _context.MarcasFavoritasCompradores.AnyAsync(mf => mf.IdComprador == user.Id && mf.IdMarca == idMarca))
            {
                _context.MarcasFavoritasCompradores.Add(new MarcaFavoritaComprador { IdComprador = user.Id, IdMarca = idMarca });
                await _context.SaveChangesAsync();
            }

            var minhasMarcas = await _context.MarcasFavoritasCompradores
                .Include(mf => mf.Marca)
                .Where(mf => mf.IdComprador == user.Id)
                .ToListAsync();

            return PartialView("_ListaMarcasPartial", minhasMarcas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverMarcaFavorita(int idMarca)
        {
            var user = await _userManager.GetUserAsync(User);

            // Procurar pelo par composto (IdComprador + IdMarca)
            var item = await _context.MarcasFavoritasCompradores
                .FirstOrDefaultAsync(mf => mf.IdComprador == user.Id && mf.IdMarca == idMarca);

            if (item != null)
            {
                _context.MarcasFavoritasCompradores.Remove(item);
                await _context.SaveChangesAsync();
            }

            var minhasMarcas = await _context.MarcasFavoritasCompradores
                .Include(mf => mf.Marca)
                .Where(mf => mf.IdComprador == user.Id)
                .ToListAsync();

            return PartialView("_ListaMarcasPartial", minhasMarcas);
        }


        [HttpGet]
        public async Task<IActionResult> ReservasVisitas()
        {
            var user = await _userManager.GetUserAsync(User);

            //Procura Reservas e transformar em "ItemAgendamento"
            var listaReservas = await _context.Reservas
                .Include(r => r.Anuncio)
                    .ThenInclude(a => a.Veiculo)
                        .ThenInclude(ve => ve.Modelo)
                .Where(r => r.IdComprador == user.Id)
                .Select(r => new ItemAgendamentoViewModel
                {
                    Id = r.IdReserva,
                    Tipo = "Reserva",
                    Estado = r.EstadoReserva,
                    DataEvento = r.PrazoExpiracao,
                    DataPedido = r.DataPedido,
                    NomeModelo = r.Anuncio.Veiculo.Modelo.Nome,
                    IdAnuncio = r.IdAnuncio
                }).ToListAsync();

            var listaVisitas = await _context.Visitas
                .Include(v => v.Anuncio)
                    .ThenInclude(a => a.Veiculo)
                        .ThenInclude(ve => ve.Modelo)
                .Where(v => v.IdComprador == user.Id)
                .Select(v => new ItemAgendamentoViewModel
                {
                    Id = v.IdVisita,
                    Tipo = "Visita",
                    Estado = v.EstadoVisita,
                    DataEvento = v.DataVisita,
                    DataPedido = v.DataPedido,
                    NomeModelo = v.Anuncio.Veiculo.Modelo.Nome,
                    IdAnuncio = v.IdAnuncio
                }).ToListAsync();


            var tudo = listaVisitas.Concat(listaReservas)
                           .OrderByDescending(x => x.DataPedido)
                           .ToList();

            var viewModel = new AgendamentosUnificadosViewModel
            {
                Ativas = tudo.Where(x => x.Estado == "Pendente" || x.Estado == "Aprovada").ToList(),
                Historico = tudo.Where(x => x.Estado != "Pendente" && x.Estado != "Aprovada").ToList()
            };

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarAgendamento(int id, string tipo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (tipo == "Reserva")
            {
                var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.IdReserva == id && r.IdComprador == user.Id);
                if (reserva != null)
                {
                    reserva.EstadoReserva = "Cancelada";
                    
                    var anuncio = await _context.Anuncios.FindAsync(reserva.IdAnuncio);
                    anuncio.Estado = "Ativo"; // Reativar o anúncio associado
                    _context.Update(reserva);
                    
                }
            }
            else if (tipo == "Visita")
            {
                var visita = await _context.Visitas.FirstOrDefaultAsync(v => v.IdVisita == id && v.IdComprador == user.Id);
                if (visita != null)
                {
                    visita.EstadoVisita = "Cancelada";
                    _context.Update(visita);
                }
            }

            await _context.SaveChangesAsync();

            
            var listaAtivas = await GetAgendamentosPorEstado(user.Id, true);
            return PartialView("_ListaReservasAtivasPartial", listaAtivas);
        }

        // Função Auxiliar para unificar as duas tabelas 
        private async Task<List<ItemAgendamentoViewModel>> GetAgendamentosPorEstado(string userId, bool apenasAtivas)
        {
            
            var visitas = await _context.Visitas
                .Include(v => v.Anuncio).ThenInclude(a => a.Veiculo).ThenInclude(ve => ve.Modelo)
                .Where(v => v.IdComprador == userId)
                .Select(v => new ItemAgendamentoViewModel
                {
                    Id = v.IdVisita,
                    Tipo = "Visita",
                    Estado = v.EstadoVisita,
                    DataEvento = v.DataVisita,
                    DataPedido = v.DataPedido,
                    NomeModelo = v.Anuncio.Veiculo.Modelo.Nome
                }).ToListAsync();

            
            var reservas = await _context.Reservas
                .Include(r => r.Anuncio).ThenInclude(a => a.Veiculo).ThenInclude(ve => ve.Modelo)
                .Where(r => r.IdComprador == userId)
                .Select(r => new ItemAgendamentoViewModel
                {
                    Id = r.IdReserva,
                    Tipo = "Reserva",
                    Estado = r.EstadoReserva,
                    DataEvento = r.PrazoExpiracao,
                    DataPedido = r.DataPedido,
                    NomeModelo = r.Anuncio.Veiculo.Modelo.Nome
                }).ToListAsync();

            var tudo = visitas.Concat(reservas).OrderByDescending(x => x.DataPedido);

            if (apenasAtivas)
                return tudo.Where(x => x.Estado == "Pendente" || x.Estado == "Aprovada").ToList();

            return tudo.Where(x => x.Estado != "Pendente" && x.Estado != "Aprovada").ToList();
        }

      
        public async Task<IActionResult> Notificacoes()
        {
            var user = await _userManager.GetUserAsync(User);

            var configs = await _context.ConfNotificacoes
                .FirstOrDefaultAsync(n => n.IdUtilizador == user.Id);

            // Se o registo ainda não existir na tabela, criamos um por defeito
            if (configs == null)
            {
                configs = new ConfNotificacoes { IdUtilizador = user.Id, AnunciosMarcasFavoritas = true, AlteracoesReservasVisitas = true };
                _context.ConfNotificacoes.Add(configs);
                await _context.SaveChangesAsync();
            }

            return View(configs);
        }

        
        [HttpPost]
        public async Task<IActionResult> AtualizarDefinicao(string campo, bool valor)
        {
            var user = await _userManager.GetUserAsync(User);
            var configs = await _context.ConfNotificacoes
                .FirstOrDefaultAsync(n => n.IdUtilizador == user.Id);

            if (configs != null)
            {
                if (campo == "marcas") configs.AnunciosMarcasFavoritas = valor;
                else if (campo == "reservas") configs.AlteracoesReservasVisitas = valor;

                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> PesquisasGuardadas()
        {
            // Em vez de escreveres a lógica toda aqui, chamas o método auxiliar
            // Mas como este é o GET inicial, queremos a View completa
            var resultado = await CarregarListaFiltros() as PartialViewResult;
            return View(resultado.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApagarFiltro(int id)
        {
            var filtro = await _context.FiltrosGuardados.FindAsync(id);
            if(filtro != null)
            {
                _context.FiltrosGuardados.Remove(filtro);
                await _context.SaveChangesAsync();
            }

            return await CarregarListaFiltros();
        }

        // Método auxiliar privado para carregar os dados e devolver a Partial View
        private async Task<IActionResult> CarregarListaFiltros()
        {
            var user = await _userManager.GetUserAsync(User);

            var filtros = await _context.FiltrosGuardados
                .Include(f => f.Marca)
                .Include(f => f.Modelo)
                .Include(f => f.Categoria)
                .Include(f => f.Combustivel)
                .Include(f => f.Caixa)
                .Include(f => f.Cor)
                .Where(f => f.IdUtilizador == user.Id)
                .OrderByDescending(f => f.DataCriacao)
                .ToListAsync();

            var viewModel = filtros.Select(f => new PesquisaGuardadaViewmodel
            {
                IdFiltro = f.IdFiltro,
                NomeFiltro = f.NomeFiltro,
                DataCriacao = f.DataCriacao,
                Badges = ConstruirBadges(f)
            }).ToList();

            return PartialView("_ListaFiltrosPartial", viewModel);
        }

        private List<string> ConstruirBadges(FiltroGuardado f)
        {
            var b = new List<string>();
            if (f.Marca != null) b.Add($"Marca: {f.Marca.Nome}");
            if (f.Modelo != null) b.Add($"Modelo: {f.Modelo.Nome}");
            if (f.PrecoMaximo > 0) b.Add($"Preço: < {f.PrecoMaximo:N0}€");
            if (f.KmMaximo > 0) b.Add($"Km: < {f.KmMaximo:N0}");
            if (f.Categoria != null) b.Add($"Categoria: {f.Categoria.Nome}");
            return b;
        }

        [HttpGet]
        public async Task<IActionResult> MinhasCompras()
        {
            var user = await _userManager.GetUserAsync(User);

            // Consulta manual para encontrar a imagem
            var comprasQuery = from c in _context.Compras
                               join a in _context.Anuncios on c.IdAnuncio equals a.IdAnuncio
                               join v in _context.Veiculos on a.IdVeiculo equals v.IdVeiculo
                               where c.IdComprador == user.Id
                               select new CompraViewModel
                               {
                                   IdCompra = c.IdCompra,
                                   DataCompra = c.DataCompra,
                                   ValorTotal = c.ValorTotal,
                                   NomeVeiculo = v.Modelo.Marca.Nome + " " + v.Modelo.Nome,
                                   // Procurar a primeira imagem do veículo
                                   ImagemUrl = _context.Imagens
                                               .Where(img => img.IdVeiculo == v.IdVeiculo)
                                               .Select(img => img.CaminhoFicheiro)
                                               .FirstOrDefault()
                               };

            var listaCompras = await comprasQuery.OrderByDescending(c => c.DataCompra).ToListAsync();

            return View(listaCompras);
        }


        
        public async Task<IActionResult> VerFatura(int id) {
            var userId = _userManager.GetUserId(User);

            var compra = await _context.Compras
                .Include(c => c.Anuncio)
                    .ThenInclude(a => a.Veiculo)
                        .ThenInclude(v => v.Modelo)
                            .ThenInclude(m => m.Marca)
                .Include(c => c.Anuncio.Vendedor)
                    .ThenInclude(v => v.user) // Para dados do vendedor
                .FirstOrDefaultAsync(c => c.IdCompra == id && c.IdComprador == userId);

            if (compra == null) {
                return NotFound("Fatura não encontrada.");
            }

            return View(compra);
        }
    }
}
