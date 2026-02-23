using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Data;
using ProjetoLAWBD.Models;
using ProjetoLAWBD.Services;
using ProjetoLAWBD.Filters;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<CheckVendedorPendente>();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CheckVendedorPendente>();
});

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;  // confirmacao de email
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvxwyzABCDEFGHIJKLMNOPQRSTUVXWYZ_-1234567890 ";

})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<PortugueseIdentityErrorDescriber>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddHttpClient(); // Adiciona o IHttpClientFactory
builder.Services.AddScoped<IReCAPTCHAService, ReCAPTCHAService>(); // Regista o serviço ReCaptcha

builder.Services.AddTransient<TestDataSeeder>();

builder.Services.AddHostedService<ReservaExpiracaoWorker>();

var configuration = builder.Configuration;

builder.Services.AddAuthentication()
   .AddGoogle(googleOptions => {
       googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
       googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
   })
   .AddTwitter(twitterOptions => {
       twitterOptions.ConsumerKey = configuration["Authentication:Twitter:ConsumerAPIKey"];
       twitterOptions.ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"];

       twitterOptions.RetrieveUserDetails = true;
   });


    

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        // 1. Semear ROLES (Crítico: Tem de ser o primeiro)
        // O TestDataSeeder precisa que "Vendedor" e "Comprador" já existam
        logger.LogInformation("A verificar Roles...");
        await RoleSeeder.SeedRolesAsync(roleManager);
        logger.LogInformation("Roles verificadas.");

        // 2. Semear ADMIN (Dados de Sistema)
        var config = services.GetRequiredService<IConfiguration>();
        logger.LogInformation("A verificar Administrador...");
        await RoleSeeder.SeedAdminUserAsync(userManager, roleManager, config, context);

        // 3. Semear DADOS DE TESTE + LOOKUPS (Marcas, Modelos, Users Teste)
        // Apenas em ambiente de desenvolvimento para não sujar a BD de produção
        if (app.Environment.IsDevelopment()) {
            logger.LogInformation("Ambiente de Desenvolvimento: A executar TestDataSeeder...");
            var testDataSeeder = services.GetRequiredService<TestDataSeeder>();
            await testDataSeeder.SeedAsync();
            logger.LogInformation("Dados de teste inseridos com sucesso.");
        }
    } catch (Exception ex) {
        logger.LogError(ex, "ERRO CRÍTICO durante a inicialização da Base de Dados.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
