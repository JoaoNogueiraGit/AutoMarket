using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Data;

namespace ProjetoLAWBD.Services
{
    public class ReservaExpiracaoWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservaExpiracaoWorker> _logger;

        public ReservaExpiracaoWorker(IServiceProvider serviceProvider, ILogger<ReservaExpiracaoWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de Expiração de Reservas iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        _logger.LogInformation("A verificar reservas expiradas às: {time}", DateTime.Now);

                        // 1. Procurar reservas que passaram do prazo e ainda estão "Aprovadas"
                        var agora = DateTime.Now;
                        var expiradas = await _context.Reservas
                            .Include(r => r.Anuncio)
                            .Where(r => r.EstadoReserva == "Aprovada" && r.PrazoExpiracao < agora)
                            .ToListAsync();

                        if (expiradas.Any())
                        {
                            foreach (var r in expiradas)
                            {
                                r.EstadoReserva = "Expirada"; // Estado que aparece no mockup
                                if (r.Anuncio != null)
                                {
                                    r.Anuncio.Estado = "Ativo"; // Liberta o carro para outros
                                }
                                _logger.LogWarning("Reserva {id} expirou. Anúncio {a} voltou a Ativo.", r.IdReserva, r.IdAnuncio);
                            }

                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar expiração de reservas.");
                }

                // 2. ESPERA: Muda para 1 minuto para testares. 
                // Se puseres 30 minutos, vais achar que não funciona porque demora muito.
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}