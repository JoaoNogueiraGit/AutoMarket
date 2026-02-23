using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.Data {
    public class TestDataSeeder {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public TestDataSeeder(UserManager<User> userManager, ApplicationDbContext context) {
            _userManager = userManager;
            _context = context;
        }

        public async Task SeedAsync() {

            // ==============================================================================
            // 1. UTILIZADORES DE TESTE
            // ==============================================================================

            // Vendedor
            var vendedorUser = await _userManager.FindByEmailAsync("vendedor.teste@automarket.com");
            if (vendedorUser == null) {
                vendedorUser = new User {
                    UserName = "vendedor_teste",
                    Email = "vendedor.teste@automarket.com",
                    EmailConfirmed = true,
                    NomeCompleto = "João Ratão"
                };
                await _userManager.CreateAsync(vendedorUser, "Password123!");
                await _userManager.AddToRoleAsync(vendedorUser, "Vendedor");

                var vendedorPerfil = new Vendedor {
                    IdUtilizador = vendedorUser.Id,
                    NIF = "123456789",
                    TipoVendedor = "Particular",
                    EstadoValidacao = "Aprovado"
                };
                await _context.Vendedores.AddAsync(vendedorPerfil);
                await _context.SaveChangesAsync();
            }

            // Comprador
            var compradorUser = await _userManager.FindByEmailAsync("comprador.teste@automarket.com");
            if (compradorUser == null) {
                compradorUser = new User {
                    UserName = "comprador_teste",
                    Email = "comprador.teste@automarket.com",
                    EmailConfirmed = true,
                    NomeCompleto = "Carlos Mendes"
                };
                await _userManager.CreateAsync(compradorUser, "Password123!");
                await _userManager.AddToRoleAsync(compradorUser, "Comprador");

                var compradorPerfil = new Comprador { IdUtilizador = compradorUser.Id };
                await _context.Compradores.AddAsync(compradorPerfil);
                await _context.SaveChangesAsync();
            }


            // ==============================================================================
            // 2. DADOS DE LOOKUP (MARCAS, MODELOS, ETC.)
            // ==============================================================================

            // --------------------- MARCAS ---------------------
            if (!_context.Marcas.Any()) {
                var marcas = new List<Marca>
                {
                    // Alemãs
                    new Marca { Nome = "BMW" },
                    new Marca { Nome = "Mercedes-Benz" },
                    new Marca { Nome = "Audi" },
                    new Marca { Nome = "Volkswagen" },
                    new Marca { Nome = "Porsche" },
                    new Marca { Nome = "Opel" },
                    // Francesas
                    new Marca { Nome = "Peugeot" },
                    new Marca { Nome = "Renault" },
                    new Marca { Nome = "Citroën" },
                    // Japonesas / Coreanas
                    new Marca { Nome = "Toyota" },
                    new Marca { Nome = "Nissan" },
                    new Marca { Nome = "Honda" },
                    new Marca { Nome = "Hyundai" },
                    new Marca { Nome = "Kia" },
                    new Marca { Nome = "Mazda" },
                    // Luxo e Supercarros
                    new Marca { Nome = "Ferrari" },
                    new Marca { Nome = "Lamborghini" },
                    new Marca { Nome = "Bentley" },
                    new Marca { Nome = "Rolls-Royce" },
                    new Marca { Nome = "Aston Martin" },
                    new Marca { Nome = "Maserati" },
                    new Marca { Nome = "McLaren" },
                    // Outras
                    new Marca { Nome = "Ford" },
                    new Marca { Nome = "Fiat" },
                    new Marca { Nome = "Seat" },
                    new Marca { Nome = "Skoda" },
                    new Marca { Nome = "Volvo" },
                    new Marca { Nome = "Tesla" },
                    new Marca { Nome = "Land Rover" },
                    new Marca { Nome = "Mini" },
                    new Marca { Nome = "Jaguar" },
                    new Marca { Nome = "Lexus" }
                };
                _context.Marcas.AddRange(marcas);
                _context.SaveChanges(); // Salvar para gerar IDs e poder usar abaixo

                // --------------------- MODELOS ---------------------
                var modelos = new List<Modelo>();
                int GetId(string nomeMarca) => marcas.First(m => m.Nome == nomeMarca).IdMarca;

                modelos.AddRange(new List<Modelo> {
                    // --- LUXO E SUPERCARROS ---
                    new Modelo { Nome = "F8 Tributo", IdMarca = GetId("Ferrari") },
                    new Modelo { Nome = "Roma", IdMarca = GetId("Ferrari") },
                    new Modelo { Nome = "Portofino", IdMarca = GetId("Ferrari") },
                    new Modelo { Nome = "812 Superfast", IdMarca = GetId("Ferrari") },
                    new Modelo { Nome = "Purosangue", IdMarca = GetId("Ferrari") },

                    new Modelo { Nome = "Huracán", IdMarca = GetId("Lamborghini") },
                    new Modelo { Nome = "Urus", IdMarca = GetId("Lamborghini") },
                    new Modelo { Nome = "Aventador", IdMarca = GetId("Lamborghini") },
                    new Modelo { Nome = "Revuelto", IdMarca = GetId("Lamborghini") },
                    new Modelo { Nome = "Gallardo", IdMarca = GetId("Lamborghini") },

                    new Modelo { Nome = "Continental GT", IdMarca = GetId("Bentley") },
                    new Modelo { Nome = "Bentayga", IdMarca = GetId("Bentley") },

                    new Modelo { Nome = "Phantom", IdMarca = GetId("Rolls-Royce") },
                    new Modelo { Nome = "Ghost", IdMarca = GetId("Rolls-Royce") },
                    new Modelo { Nome = "Cullinan", IdMarca = GetId("Rolls-Royce") },

                    new Modelo { Nome = "DB11", IdMarca = GetId("Aston Martin") },
                    new Modelo { Nome = "Vantage", IdMarca = GetId("Aston Martin") },
                    new Modelo { Nome = "DBX", IdMarca = GetId("Aston Martin") },

                    new Modelo { Nome = "Ghibli", IdMarca = GetId("Maserati") },
                    new Modelo { Nome = "Levante", IdMarca = GetId("Maserati") },
                    new Modelo { Nome = "MC20", IdMarca = GetId("Maserati") },

                    new Modelo { Nome = "720S", IdMarca = GetId("McLaren") },
                    new Modelo { Nome = "Artura", IdMarca = GetId("McLaren") },

                    new Modelo { Nome = "F-Type", IdMarca = GetId("Jaguar") },
                    new Modelo { Nome = "F-Pace", IdMarca = GetId("Jaguar") },
                    new Modelo { Nome = "I-Pace", IdMarca = GetId("Jaguar") },

                    new Modelo { Nome = "NX", IdMarca = GetId("Lexus") },
                    new Modelo { Nome = "RX", IdMarca = GetId("Lexus") },
                    new Modelo { Nome = "LC", IdMarca = GetId("Lexus") },

                    // --- MARCAS GERAIS ---
                    // BMW
                    new Modelo { Nome = "Série 1", IdMarca = GetId("BMW") },
                    new Modelo { Nome = "Série 3", IdMarca = GetId("BMW") },
                    new Modelo { Nome = "320d M Sport", IdMarca = GetId("BMW") },
                    new Modelo { Nome = "Série 5", IdMarca = GetId("BMW") },
                    new Modelo { Nome = "X1", IdMarca = GetId("BMW") },
                    new Modelo { Nome = "X3", IdMarca = GetId("BMW") },
                    new Modelo { Nome = "X5", IdMarca = GetId("BMW") },
                    new Modelo { Nome = "M3", IdMarca = GetId("BMW") },
                    new Modelo { Nome = "M4", IdMarca = GetId("BMW") },

                    // Mercedes-Benz
                    new Modelo { Nome = "Classe A", IdMarca = GetId("Mercedes-Benz") },
                    new Modelo { Nome = "Classe C", IdMarca = GetId("Mercedes-Benz") },
                    new Modelo { Nome = "C200", IdMarca = GetId("Mercedes-Benz") },
                    new Modelo { Nome = "Classe E", IdMarca = GetId("Mercedes-Benz") },
                    new Modelo { Nome = "CLA", IdMarca = GetId("Mercedes-Benz") },
                    new Modelo { Nome = "GLA", IdMarca = GetId("Mercedes-Benz") },
                    new Modelo { Nome = "GLC", IdMarca = GetId("Mercedes-Benz") },
                    new Modelo { Nome = "Classe G", IdMarca = GetId("Mercedes-Benz") },

                    // Audi
                    new Modelo { Nome = "A1", IdMarca = GetId("Audi") },
                    new Modelo { Nome = "A3", IdMarca = GetId("Audi") },
                    new Modelo { Nome = "A4", IdMarca = GetId("Audi") },
                    new Modelo { Nome = "A5", IdMarca = GetId("Audi") },
                    new Modelo { Nome = "A6", IdMarca = GetId("Audi") },
                    new Modelo { Nome = "Q3", IdMarca = GetId("Audi") },
                    new Modelo { Nome = "Q5", IdMarca = GetId("Audi") },
                    new Modelo { Nome = "Q8", IdMarca = GetId("Audi") },
                    new Modelo { Nome = "e-tron GT", IdMarca = GetId("Audi") },

                    // Porsche
                    new Modelo { Nome = "911", IdMarca = GetId("Porsche") },
                    new Modelo { Nome = "Cayenne", IdMarca = GetId("Porsche") },
                    new Modelo { Nome = "Panamera", IdMarca = GetId("Porsche") },
                    new Modelo { Nome = "Macan", IdMarca = GetId("Porsche") },
                    new Modelo { Nome = "Taycan", IdMarca = GetId("Porsche") },

                    // Tesla
                    new Modelo { Nome = "Model 3", IdMarca = GetId("Tesla") },
                    new Modelo { Nome = "Model Y", IdMarca = GetId("Tesla") },
                    new Modelo { Nome = "Model S", IdMarca = GetId("Tesla") },
                    new Modelo { Nome = "Model X", IdMarca = GetId("Tesla") },

                    // Land Rover
                    new Modelo { Nome = "Range Rover", IdMarca = GetId("Land Rover") },
                    new Modelo { Nome = "Range Rover Sport", IdMarca = GetId("Land Rover") },
                    new Modelo { Nome = "Evoque", IdMarca = GetId("Land Rover") },
                    new Modelo { Nome = "Defender", IdMarca = GetId("Land Rover") },

                    // Volkswagen
                    new Modelo { Nome = "Golf", IdMarca = GetId("Volkswagen") },
                    new Modelo { Nome = "Passat", IdMarca = GetId("Volkswagen") },
                    new Modelo { Nome = "Tiguan", IdMarca = GetId("Volkswagen") },
                    new Modelo { Nome = "T-Roc", IdMarca = GetId("Volkswagen") },

                    // Peugeot
                    new Modelo { Nome = "208", IdMarca = GetId("Peugeot") },
                    new Modelo { Nome = "3008", IdMarca = GetId("Peugeot") },
                    new Modelo { Nome = "508", IdMarca = GetId("Peugeot") },

                    // Renault
                    new Modelo { Nome = "Clio", IdMarca = GetId("Renault") },
                    new Modelo { Nome = "Megane", IdMarca = GetId("Renault") },
                    new Modelo { Nome = "Captur", IdMarca = GetId("Renault") },

                    // Toyota
                    new Modelo { Nome = "Yaris", IdMarca = GetId("Toyota") },
                    new Modelo { Nome = "Corolla", IdMarca = GetId("Toyota") },
                    new Modelo { Nome = "RAV4", IdMarca = GetId("Toyota") },

                    // Nissan
                    new Modelo { Nome = "Qashqai", IdMarca = GetId("Nissan") },
                    new Modelo { Nome = "Juke", IdMarca = GetId("Nissan") },

                    // Ford
                    new Modelo { Nome = "Focus", IdMarca = GetId("Ford") },
                    new Modelo { Nome = "Fiesta", IdMarca = GetId("Ford") },
                    new Modelo { Nome = "Mustang", IdMarca = GetId("Ford") },

                    // Fiat
                    new Modelo { Nome = "500", IdMarca = GetId("Fiat") },
                    new Modelo { Nome = "Panda", IdMarca = GetId("Fiat") },
                    new Modelo { Nome = "Tipo", IdMarca = GetId("Fiat") },

                    // Volvo
                    new Modelo { Nome = "XC40", IdMarca = GetId("Volvo") },
                    new Modelo { Nome = "XC60", IdMarca = GetId("Volvo") },
                    new Modelo { Nome = "XC90", IdMarca = GetId("Volvo") },

                    // Mini
                    new Modelo { Nome = "Cooper", IdMarca = GetId("Mini") },
                    new Modelo { Nome = "Countryman", IdMarca = GetId("Mini") }
                });

                _context.Modelos.AddRange(modelos);
                _context.SaveChanges();
            }

            // --------------------- CORES ---------------------
            if (!_context.Cores.Any()) {
                var cores = new List<Cor>
                {
                    new Cor { Nome = "Preto" },
                    new Cor { Nome = "Branco" },
                    new Cor { Nome = "Cinzento" },
                    new Cor { Nome = "Azul" },
                    new Cor { Nome = "Vermelho" },
                    new Cor { Nome = "Prateado" },
                    new Cor { Nome = "Verde" },
                    new Cor { Nome = "Amarelo" },
                    new Cor { Nome = "Castanho" },
                    new Cor { Nome = "Laranja" },
                    new Cor { Nome = "Bege" },
                    new Cor { Nome = "Dourado" },
                    new Cor { Nome = "Mate" }
                };
                _context.Cores.AddRange(cores);
                _context.SaveChanges();
            }

            // --------------------- CAIXAS ---------------------
            if (!_context.Caixas.Any()) {
                var caixas = new List<Caixa>
                {
                    new Caixa { Nome = "Manual" },
                    new Caixa { Nome = "Automática" },
                    new Caixa { Nome = "Semi-Automática" },
                    new Caixa { Nome = "CVT" }
                };
                _context.Caixas.AddRange(caixas);
                _context.SaveChanges();
            }

            // --------------------- CATEGORIAS ---------------------
            if (!_context.Categorias.Any()) {
                var categorias = new List<Categoria>
                {
                    new Categoria { Nome = "Sedan" },
                    new Categoria { Nome = "SUV" },
                    new Categoria { Nome = "Citadino" },
                    new Categoria { Nome = "Hatchback" },
                    new Categoria { Nome = "Carrinha" },
                    new Categoria { Nome = "Cabrio" },
                    new Categoria { Nome = "Coupé" },
                    new Categoria { Nome = "Monovolume" },
                    new Categoria { Nome = "Pickup" },
                    new Categoria { Nome = "Todo-o-Terreno" }
                };
                _context.Categorias.AddRange(categorias);
                _context.SaveChanges();
            }

            // --------------------- COMBUSTÍVEIS ---------------------
            if (!_context.Combustiveis.Any()) {
                var combustiveis = new List<Combustivel>
                {
                    new Combustivel { Nome = "Gasolina" },
                    new Combustivel { Nome = "Diesel" },
                    new Combustivel { Nome = "GPL" },
                    new Combustivel { Nome = "Elétrico" },
                    new Combustivel { Nome = "Híbrido (Gasolina)" },
                    new Combustivel { Nome = "Híbrido (Diesel)" },
                    new Combustivel { Nome = "Híbrido Plug-in" }
                };
                _context.Combustiveis.AddRange(combustiveis);
                _context.SaveChanges();
            }

            // ==============================================================================
            // 3. VEÍCULOS E ANÚNCIOS DE EXEMPLO
            // ==============================================================================
            if (!await _context.Anuncios.AnyAsync()) {

                // Buscar Referências (Usamos FirstOrDefault para segurança)
                var modeloBmw = await _context.Modelos.FirstOrDefaultAsync(m => m.Nome == "320d M Sport")
                             ?? await _context.Modelos.FirstOrDefaultAsync(m => m.Nome == "Série 3");

                var modeloMercedes = await _context.Modelos.FirstOrDefaultAsync(m => m.Nome == "C200")
                                  ?? await _context.Modelos.FirstOrDefaultAsync(m => m.Nome == "Classe C");

                var corPreto = await _context.Cores.FirstAsync();
                var caixaAuto = await _context.Caixas.FirstAsync();
                var catSedan = await _context.Categorias.FirstAsync();
                var combDiesel = await _context.Combustiveis.FirstAsync();

                if (modeloBmw != null && modeloMercedes != null) {
                    // --- Veículo 1 (BMW) ---
                    var veiculo1 = new Veiculo {
                        IdVendedor = vendedorUser.Id,
                        IdModelo = modeloBmw.IdModelo,
                        IdCor = corPreto.IdCor,
                        IdCaixa = caixaAuto.IdCaixa,
                        IdCombustivel = combDiesel.IdCombustivel,
                        IdCategoria = catSedan.IdCategoria,
                        PotenciaCV = 190,
                        NumLugares = 5,
                        NumPortas = 4,
                        DataMatricula = new DateTime(2019, 5, 1),
                        KmTotal = 45000
                    };
                    await _context.Veiculos.AddAsync(veiculo1);

                    // --- Veículo 2 (Mercedes) ---
                    var veiculo2 = new Veiculo {
                        IdVendedor = vendedorUser.Id,
                        IdModelo = modeloMercedes.IdModelo,
                        IdCor = corPreto.IdCor,
                        IdCaixa = caixaAuto.IdCaixa,
                        IdCombustivel = combDiesel.IdCombustivel,
                        IdCategoria = catSedan.IdCategoria,
                        PotenciaCV = 180,
                        NumLugares = 5,
                        NumPortas = 4,
                        DataMatricula = new DateTime(2021, 1, 10),
                        KmTotal = 22000
                    };
                    await _context.Veiculos.AddAsync(veiculo2);
                    await _context.SaveChangesAsync();

                    // --- Anúncios ---
                    var anuncio1 = new Anuncio {
                        IdVendedor = vendedorUser.Id,
                        IdVeiculo = veiculo1.IdVeiculo,
                        Titulo = "BMW Série 3 Desportivo (Como Novo)",
                        Preco = 28500.00m,
                        LocalizacaoCidade = "Lisboa",
                        Estado = "Ativo",
                        DataPublicacao = DateTime.UtcNow.AddDays(-5)
                    };
                    var anuncio2 = new Anuncio {
                        IdVendedor = vendedorUser.Id,
                        IdVeiculo = veiculo1.IdVeiculo, // Mesmo veículo, mas vendido antes
                        Titulo = "BMW Série 3 (Oportunidade)",
                        Preco = 29000.00m,
                        LocalizacaoCidade = "Lisboa",
                        Estado = "Vendido",
                        DataPublicacao = DateTime.UtcNow.AddDays(-60)
                    };
                    var anuncio3 = new Anuncio {
                        IdVendedor = vendedorUser.Id,
                        IdVeiculo = veiculo2.IdVeiculo,
                        Titulo = "Mercedes-Benz Classe C Impecável",
                        Preco = 32000.00m,
                        LocalizacaoCidade = "Porto",
                        Estado = "Pausado",
                        DataPublicacao = DateTime.UtcNow.AddDays(-10)
                    };
                    await _context.Anuncios.AddRangeAsync(anuncio1, anuncio2, anuncio3);
                    await _context.SaveChangesAsync();

                    // --- Pedidos (Visitas e Reservas) ---
                    var visita1 = new Visita {
                        IdComprador = compradorUser.Id,
                        IdAnuncio = anuncio1.IdAnuncio,
                        EstadoVisita = "Pendente",
                        DataPedido = DateTime.UtcNow.AddDays(-1),
                        DataVisita = DateTime.UtcNow.AddDays(3)
                    };
                    var reserva1 = new Reserva {
                        IdComprador = compradorUser.Id,
                        IdAnuncio = anuncio3.IdAnuncio,
                        EstadoReserva = "Pendente",
                        DataPedido = DateTime.UtcNow.AddDays(-1)
                    };
                    await _context.Visitas.AddAsync(visita1);
                    await _context.Reservas.AddAsync(reserva1);

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}