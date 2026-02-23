using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.ViewModels;
using SQLitePCL;

namespace ProjetoLAWBD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var destaques = await _context.Anuncios
                            .Include(a => a.Veiculo).ThenInclude(v => v.Modelo).ThenInclude(m => m.Marca)
                            .Include(a => a.Veiculo.Combustivel)
                            .Where(a => a.Estado == "Ativo")
                            .OrderByDescending(a => a.DataPublicacao)
                            .Take(6)
                            .ToListAsync();

            var veiculosIDs = destaques.Select(a => a.IdVeiculo).Distinct().ToList();

            var imagens = await _context.Imagens
                            .Where(i => veiculosIDs.Contains(i.IdVeiculo))
                            .ToListAsync();

            var maxPrice = await _context.Anuncios.MaxAsync(a => a.Preco);

            var viewModel = new HomeIndexViewModel {

                VeiculosEmDestaque = destaques,
                ImagensDosVeiculos = imagens,
                Marcas = await _context.Marcas.ToListAsync(),
                Modelos = await _context.Modelos.ToListAsync(),
                Caixas = await _context.Caixas.ToListAsync(),
                Combustiveis = await _context.Combustiveis.ToListAsync(),
                Cores = await _context.Cores.ToListAsync(),
                Categorias = await _context.Categorias.ToListAsync(),
                PrecoMaxDb = _context.Anuncios.Any() ? Math.Ceiling(maxPrice / 5000) * 5000 : 2000000

            };

            if(User.Identity.IsAuthenticated) {

                var userId = _userManager.GetUserId(User);
                viewModel.FiltrosGuardados = await _context.FiltrosGuardados
                                                    .Where(f => f.IdUtilizador == userId)
                                                    .OrderByDescending(f => f.DataCriacao)
                                                    .ToListAsync();
            }



            return View(viewModel);
        }

        public IActionResult Sobre() {
            return View();
        }

        public IActionResult Ajuda() {
            return View();
        }

        public IActionResult Privacidade()
        {
            return View();
        }

        public IActionResult Termos() {
            return View();
        }

        public IActionResult Contactos() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
