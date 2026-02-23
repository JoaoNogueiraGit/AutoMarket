using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.ViewModels;
using System.Linq; 
using System.Threading.Tasks;

namespace ProjetoLAWBD.Controllers {

    
    public class AnunciosController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;

        public AnunciosController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment env) {
            _context = context;
            _userManager = userManager;
            _env = env;
        }


        [HttpGet]
        public async Task<IActionResult> Detalhes(int id) {
            // Vai buscar o anúncio e TODOS os seus dados relacionados
            var anuncio = await _context.Anuncios
                .Include(a => a.Veiculo)
                    .ThenInclude(v => v.Modelo)
                        .ThenInclude(m => m.Marca)
                .Include(a => a.Veiculo.Combustivel)
                .Include(a => a.Veiculo.Cor)
                .Include(a => a.Veiculo.Caixa)
                .Include(a => a.Veiculo.Categoria)
                .Include(a => a.Vendedor)
                    .ThenInclude(v => v.user)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id);

            // Verifica se o anúncio existe E se está visível
            if (anuncio == null ||
               (anuncio.Estado != "Ativo" && anuncio.Estado != "Reservado")) {
                return NotFound("Anúncio não encontrado ou já não está disponível.");
            }

            // Vai buscar as imagens Ordenadas pela 'OrdemExibicao'
            var imagensDoVeiculo = await _context.Imagens
                                        .Where(i => i.IdVeiculo == anuncio.IdVeiculo)
                                        .OrderBy(i => i.OrdemExibicao)
                                        .ToListAsync();

            // buscar contactos do vendedor
            var contactosVendedor = await _context.Contactos
                                            .Where(c => c.IdUtilizador == anuncio.IdVendedor)
                                            .ToListAsync();

            // Prepara o ViewModel
            var viewModel = new AnuncioDetalhesViewModel {
                Anuncio = anuncio,
                Imagens = imagensDoVeiculo,
                ContactosVendedor = contactosVendedor
            };

            return View(viewModel);
        }

        // --- A PÁGINA DE PESQUISA (para o link "Voltar") ---
        // GET: /Anuncios/Index
        [HttpGet]
        public async Task<IActionResult> Index(AnunciosIndexViewModel filtros) {
            
            // Garantir que não há números negativos
            if (filtros.PrecoMin.HasValue && filtros.PrecoMin < 0) filtros.PrecoMin = 0;
            if (filtros.PrecoMax.HasValue && filtros.PrecoMax < 0) filtros.PrecoMax = 0;
            if (filtros.KmsAte.HasValue && filtros.KmsAte < 0) filtros.KmsAte = 0;
            if (filtros.AnoDe.HasValue && filtros.AnoDe < 1900) filtros.AnoDe = 1900;
            if (filtros.AnoAte.HasValue && filtros.AnoAte < 1900) filtros.AnoAte = 1900;

            // Troca Inteligente: Se Mínimo > Máximo, trocamos os valores
            if (filtros.PrecoMin.HasValue && filtros.PrecoMax.HasValue && filtros.PrecoMin > filtros.PrecoMax) {
                var temp = filtros.PrecoMin;
                filtros.PrecoMin = filtros.PrecoMax;
                filtros.PrecoMax = temp;
            }

            // query base
            var query = _context.Anuncios
                .Where(a => a.Estado == "Ativo")
                .Include(a => a.Veiculo).ThenInclude(v => v.Modelo).ThenInclude(m => m.Marca)
                .Include(a => a.Veiculo).ThenInclude(v => v.Combustivel)
                .Include(a => a.Veiculo).ThenInclude(v => v.Caixa)
                .Include(a => a.Veiculo).ThenInclude(v => v.Categoria) // Importante incluir Categoria
                .Include(a => a.Veiculo).ThenInclude(v => v.Cor)
                .AsQueryable();

            
            // Aplicar filtros

            // Pesquisa Texto
            if (!string.IsNullOrEmpty(filtros.Termo)) {

                query = query.Where(a => a.Titulo.Contains(filtros.Termo) ||
                                         a.Veiculo.Modelo.Nome.Contains(filtros.Termo) ||
                                         a.Veiculo.Modelo.Marca.Nome.Contains(filtros.Termo));
            }

            // LÓGICA DE LOCALIZAÇÃO
            bool aplicouFiltroRaio = false;

            // Se temos coordenadas e um raio válido, filtramos pela distância
            if (filtros.Latitude.HasValue && filtros.Longitude.HasValue && filtros.Raio.HasValue && filtros.Raio.Value > 0) {
                var cidadesNoRaio = ObterCidadesNoRaio(filtros.Latitude.Value, filtros.Longitude.Value, filtros.Raio.Value);

                if (cidadesNoRaio.Any()) {
                    query = query.Where(a => cidadesNoRaio.Contains(a.LocalizacaoCidade));
                    aplicouFiltroRaio = true;
                }
            }

            // Se NÃO aplicou raio, usa filtro de texto normal
            if (!aplicouFiltroRaio && !string.IsNullOrEmpty(filtros.Localizacao)) {
                query = query.Where(a => a.LocalizacaoCidade.Contains(filtros.Localizacao));
            }
            // ---------------------------------------------

            // Filtros de IDs (Dropdowns)
            if (filtros.MarcaId.HasValue)
                query = query.Where(a => a.Veiculo.Modelo.IdMarca == filtros.MarcaId);

            if (filtros.ModeloId.HasValue)
                query = query.Where(a => a.Veiculo.IdModelo == filtros.ModeloId);

            if (filtros.CategoriaId.HasValue)
                query = query.Where(a => a.Veiculo.IdCategoria == filtros.CategoriaId);

            if (filtros.CombustivelId.HasValue)
                query = query.Where(a => a.Veiculo.IdCombustivel == filtros.CombustivelId);

            if (filtros.CaixaId.HasValue)
                query = query.Where(a => a.Veiculo.IdCaixa == filtros.CaixaId);

            if (filtros.CorId.HasValue)
                query = query.Where(a => a.Veiculo.IdCor == filtros.CorId);

            // Filtros Numéricos
            if (filtros.KmsAte.HasValue)
                query = query.Where(a => a.Veiculo.KmTotal <= filtros.KmsAte);

            if (filtros.AnoDe.HasValue)
                query = query.Where(a => a.Veiculo.DataMatricula.Year >= filtros.AnoDe);

            if (filtros.AnoAte.HasValue)
                query = query.Where(a => a.Veiculo.DataMatricula.Year <= filtros.AnoAte);

            if (filtros.PrecoMin.HasValue)
                query = query.Where(a => a.Preco >= filtros.PrecoMin);

            if (filtros.PrecoMax.HasValue)
                query = query.Where(a => a.Preco <= filtros.PrecoMax);

            // Ordenação
            query = filtros.Ordenacao switch {
                "preco_asc" => query.OrderBy(a => a.Preco),
                "preco_desc" => query.OrderByDescending(a => a.Preco),
                "km_asc" => query.OrderBy(a => a.Veiculo.KmTotal),
                _ => query.OrderByDescending(a => a.DataPublicacao)
            };

            // Executar query
            filtros.Anuncios = await query.ToListAsync();
            filtros.TotalAnuncios = filtros.Anuncios.Count;

            // Carregar imagens
            if (filtros.Anuncios.Any()) {
                var idsVeiculos = filtros.Anuncios.Select(a => a.IdVeiculo).ToList();

                filtros.ImagensDosVeiculos = await _context.Imagens 
                    .Where(img => idsVeiculos.Contains(img.IdVeiculo))
                    .OrderBy(img => img.OrdemExibicao)
                    .ToListAsync();
            }
            else {
                filtros.ImagensDosVeiculos = new List<Imagem>();
            }

            
            // 6. CALCULAR PREÇO MÁXIMO (Para o Slider)
            // Vamos à base de dados buscar o carro mais caro (Global) para definir o teto do slider
            if (await _context.Anuncios.AnyAsync(a => a.Estado == "Ativo")) {
                decimal maxDb = await _context.Anuncios
                    .Where(a => a.Estado == "Ativo")
                    .MaxAsync(a => a.Preco);

                // Arredondar para cima (múltiplos de 1000) para ficar bonito
                filtros.PrecoMaxDb = Math.Ceiling(maxDb / 1000) * 1000;
            }
            else {
                filtros.PrecoMaxDb = 200000; // Valor fallback
            }

            // Se o utilizador não filtrou um máximo, o "selecionado" é o máximo da BD
            if (!filtros.PrecoMax.HasValue || filtros.PrecoMax == 0) {
                filtros.PrecoMax = filtros.PrecoMax;
            }

            // Preencher dropdowns
            filtros.Marcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();
            filtros.Modelos = await _context.Modelos.OrderBy(m => m.Nome).ToListAsync();
            filtros.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
            filtros.Combustiveis = await _context.Combustiveis.ToListAsync();
            filtros.Caixas = await _context.Caixas.ToListAsync();
            filtros.Cores = await _context.Cores.ToListAsync();


            

            return View(filtros);
        }

        // ===============================
        // MÉTODOS AUXILIARES PARA O RAIO 
        // ===============================

        private List<string> ObterCidadesNoRaio(double latOrigem, double lngOrigem, int raioKm) {
            try {
                string path = Path.Combine(_env.WebRootPath, "dados", "locais.json");
                if (!System.IO.File.Exists(path)) return new List<string>();

                string jsonContent = System.IO.File.ReadAllText(path);
                // Precisas de: using System.Text.Json;
                var todosLocais = System.Text.Json.JsonSerializer.Deserialize<List<LocalJsonItem>>(jsonContent);

                if (todosLocais == null) return new List<string>();

                return todosLocais
                    .Where(l => CalcularDistancia(latOrigem, lngOrigem, l.Lat, l.Lng) <= raioKm)
                    .Select(l => l.Name)
                    .ToList();
            } catch {
                return new List<string>();
            }
        }

        private double CalcularDistancia(double lat1, double lon1, double lat2, double lon2) {
            var R = 6371; // Raio da Terra em km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double angle) => Math.PI * angle / 180.0;

        // Classe simples para ler o JSON
        private class LocalJsonItem {
            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("lat")]
            public double Lat { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("lng")]
            public double Lng { get; set; }
        }



        // =========================================================
        // GUARDAR FILTRO (POST) - Com lógica de Atualização/Novo
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarFiltro(AnunciosIndexViewModel model, string nomeFiltro, int? idFiltroParaAtualizar, string acao) {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            // -----------------------------------------------------
            // CENÁRIO A: ATUALIZAR FILTRO EXISTENTE
            // -----------------------------------------------------
            if (acao == "atualizar" && idFiltroParaAtualizar.HasValue) {
                // Buscar filtro e garantir que pertence ao user (Segurança)
                var filtroExistente = await _context.FiltrosGuardados
                    .FirstOrDefaultAsync(f => f.IdFiltro == idFiltroParaAtualizar && f.IdUtilizador == userId);

                if (filtroExistente != null) {
                    // Atualizar Metadados
                    filtroExistente.NomeFiltro = nomeFiltro; // Atualiza o nome caso o user o tenha mudado
                    filtroExistente.DataCriacao = DateTime.UtcNow; // Traz para o topo da lista

                    // Atualizar Campos de Pesquisa
                    filtroExistente.Termo = model.Termo;
                    filtroExistente.Localizacao = model.Localizacao;
                    filtroExistente.Latitude = model.Latitude;
                    filtroExistente.Longitude = model.Longitude;
                    filtroExistente.Raio = model.Raio;

                    // Atualizar IDs
                    filtroExistente.IdMarca = model.MarcaId;
                    filtroExistente.IdModelo = model.ModeloId;
                    filtroExistente.IdCategoria = model.CategoriaId;
                    filtroExistente.IdCombustivel = model.CombustivelId;
                    filtroExistente.IdCaixa = model.CaixaId;
                    filtroExistente.IdCor = model.CorId;

                    // Atualizar Ranges
                    filtroExistente.AnoMinimo = model.AnoDe;
                    filtroExistente.AnoMaximo = model.AnoAte;
                    filtroExistente.KmMaximo = model.KmsAte;
                    filtroExistente.PrecoMinimo = model.PrecoMin;
                    filtroExistente.PrecoMaximo = model.PrecoMax;

                    _context.FiltrosGuardados.Update(filtroExistente);
                    await _context.SaveChangesAsync();

                    TempData["Sucesso"] = "Filtro atualizado com sucesso!";

                    // Redireciona mantendo este filtro como "Ativo"
                    return RedirecionarComFiltros(model, filtroExistente.IdFiltro, filtroExistente.NomeFiltro);
                }
            }

            // -----------------------------------------------------
            // CENÁRIO B: CRIAR NOVO FILTRO (Padrão)
            // -----------------------------------------------------
            var novoFiltro = new FiltroGuardado {
                IdUtilizador = userId,
                NomeFiltro = nomeFiltro,
                DataCriacao = DateTime.UtcNow,

                // Mapeamento Completo
                Termo = model.Termo,
                Localizacao = model.Localizacao,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Raio = model.Raio,

                IdMarca = model.MarcaId,
                IdModelo = model.ModeloId,
                IdCategoria = model.CategoriaId,
                IdCombustivel = model.CombustivelId,
                IdCaixa = model.CaixaId,
                IdCor = model.CorId,

                AnoMinimo = model.AnoDe,
                AnoMaximo = model.AnoAte,
                KmMaximo = model.KmsAte,
                PrecoMinimo = model.PrecoMin,
                PrecoMaximo = model.PrecoMax
            };

            _context.FiltrosGuardados.Add(novoFiltro);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Nova pesquisa guardada!";

            // Redireciona definindo este novo filtro como "Ativo"
            return RedirecionarComFiltros(model, novoFiltro.IdFiltro, novoFiltro.NomeFiltro);
        }

        // =========================================================
        // HELPER PRIVADO (Para não repetir código de redirect)
        // =========================================================
        private IActionResult RedirecionarComFiltros(AnunciosIndexViewModel model, int idFiltro, string nomeFiltro) {
            return RedirectToAction("Index", new {
                // Estado do Filtro Ativo
                idFiltroAtivo = idFiltro,
                nomeFiltroAtivo = nomeFiltro,

                // Dados da Pesquisa
                termo = model.Termo,
                localizacao = model.Localizacao,
                latitude = model.Latitude,
                longitude = model.Longitude,
                raio = model.Raio,
                marcaId = model.MarcaId,
                modeloId = model.ModeloId,
                categoriaId = model.CategoriaId,
                combustivelId = model.CombustivelId,
                caixaId = model.CaixaId,
                corId = model.CorId,
                anoDe = model.AnoDe,
                anoAte = model.AnoAte,
                kmsAte = model.KmsAte,
                precoMin = model.PrecoMin,
                precoMax = model.PrecoMax,
                ordenacao = model.Ordenacao
            });
        }

        [HttpGet]
        public async Task<IActionResult> CarregarFiltro(int id) {
            var userId = _userManager.GetUserId(User);
            var filtro = await _context.FiltrosGuardados
                                .FirstOrDefaultAsync(f => f.IdFiltro == id && f.IdUtilizador == userId);

            if (filtro == null) return NotFound();

            return RedirectToAction("Index", new {

                idFiltroAtivo = filtro.IdFiltro,
                nomeFiltroAtivo = filtro.NomeFiltro,

                // Novos
                termo = filtro.Termo,
                localizacao = filtro.Localizacao,
                latitude = filtro.Latitude,
                longitude = filtro.Longitude,
                raio = filtro.Raio,

                // Antigos
                marcaId = filtro.IdMarca,
                modeloId = filtro.IdModelo,
                categoriaId = filtro.IdCategoria,
                combustivelId = filtro.IdCombustivel,
                caixaId = filtro.IdCaixa,
                anoDe = filtro.AnoMinimo,
                anoAte = filtro.AnoMaximo,
                kmsAte = filtro.KmMaximo,
                precoMin = filtro.PrecoMinimo,
                precoMax = filtro.PrecoMaximo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApagarFiltro(int id) {

            var userId = _userManager.GetUserId(User);
            var filtro = await _context.FiltrosGuardados
                            .FirstOrDefaultAsync(f => f.IdFiltro == id && f.IdUtilizador == userId);

            if (filtro != null) {

                
                _context.FiltrosGuardados.Remove(filtro);
                _context.SaveChanges();
                TempData["Sucesso"] = "Filtro removido!";
            }

            // Volta para onde estava
            return Redirect(Request.Headers["Referer"].ToString());
        }


        // Metodos para reserva e visita

        [Authorize(Roles = "Comprador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int anuncioId)
        {
            var userId = _userManager.GetUserId(User);

            // Verificamos se já existe uma reserva ativa para este anúncio
            var reservaExistente = await _context.Reservas
                .AnyAsync(r => r.IdAnuncio == anuncioId && r.EstadoReserva == "Aprovada");

            if (reservaExistente)
            {
                TempData["Erro"] = "Este veículo já se encontra reservado.";
                return RedirectToAction("Detalhes", new { id = anuncioId });
            }

            var reserva = new Reserva
            {
                IdComprador = userId,
                IdAnuncio = anuncioId,
                DataPedido = DateTime.Now,
                EstadoReserva = "Pendente" 
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Pedido de reserva enviado ao vendedor. Aguarde a confirmação.";
            return RedirectToAction("ReservasVisitas", "Comprador");
        }


        [Authorize(Roles = "Comprador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Visitar(int anuncioId, DateTime dataSugerida)
        {
            var userId = _userManager.GetUserId(User);

            if (dataSugerida <= DateTime.Now)
            {
                TempData["Erro"] = "A data da visita deve ser no futuro.";
                return RedirectToAction("Detalhes", new { id = anuncioId });
            }

            var visita = new Visita
            {
                IdComprador = userId,
                IdAnuncio = anuncioId,
                DataPedido = DateTime.Now,
                DataVisita = dataSugerida,
                EstadoVisita = "Pendente" 
            };

            _context.Visitas.Add(visita);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Pedido de visita enviado! Aguarde a confirmação do vendedor.";
            return RedirectToAction("ReservasVisitas", "Comprador");
        }

    }
}