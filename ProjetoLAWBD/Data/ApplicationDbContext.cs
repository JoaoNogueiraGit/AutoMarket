using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetoLAWBD.Models;

namespace ProjetoLAWBD.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        // DbSets Principais de Utilizadores
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Comprador> Compradores { get; set; }

        // DbSets Relacionados com Anúncios e Veículos
        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Combustivel> Combustiveis { get; set; }
        public DbSet<Cor> Cores { get; set; }
        public DbSet<Imagem> Imagens { get; set; }
        public DbSet<Caixa> Caixas { get; set; } 

        // DbSets de Ações do Utilizador
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Visita> Visitas { get; set; }
        public DbSet<MarcaFavoritaComprador> MarcasFavoritasCompradores { get; set; }
        public DbSet<FiltroGuardado> FiltrosGuardados { get; set; }

        // --- DbSets de Gestão e Sistema ---
        public DbSet<Bloqueio> Bloqueios { get; set; }
        public DbSet<ConfNotificacoes> ConfNotificacoes { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<HistoricoDeAcoes> HistoricoDeAcoes { get; set; }
        public DbSet<Morada> Moradas { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            // =================================================================
            // 1. ESPECIALIZAÇÃO DE UTILIZADORES (e tabelas 1-para-1)
            // =================================================================

            // --- Utilizador ---
            builder.Entity<User>(b =>
            {
                b.Property(u => u.DataRegisto).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");
            });

            // --- Administrador ---
            builder.Entity<Administrador>().ToTable("Administrador");
            builder.Entity<Administrador>()
                .HasOne(a => a.AdminCriador)
                .WithMany()
                .HasForeignKey(a => a.IdAdministradorCriador)
                .OnDelete(DeleteBehavior.NoAction);

            // --- Vendedor ---
            builder.Entity<Vendedor>()
                .HasOne(v => v.AdministradorValidador)
                .WithMany()
                .HasForeignKey(v => v.IdAdministradorValidador)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Vendedor>()
                .Property(v => v.IdAdministradorValidador)
                .IsRequired(false);


            builder.Entity<Vendedor>()
                .Property(v => v.NIF)
                .HasColumnType("varchar(9)");

            builder.Entity<Vendedor>()
                .HasIndex(v => v.NIF)
                .IsUnique();

            builder.Entity<Vendedor>()
                .ToTable("Vendedor", t => {
                    t.HasCheckConstraint("CK_Vendedor_Tipo", "tipo_vendedor IN ('Particular', 'Empresa')");
                    t.HasCheckConstraint("CK_Vendedor_Estado", "estado_validacao IN ('Pendente', 'Aprovado', 'Rejeitado')");
                    t.HasCheckConstraint("CK_Vendedor_NIF", "nif LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'");
                    t.HasCheckConstraint("CK_Vendedor_CodigoPostal", "morada_faturacao_codigo_postal IS NULL OR morada_faturacao_codigo_postal LIKE '[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9]'");
                    t.HasCheckConstraint("CK_Vendedor_IBAN", "iban IS NULL OR iban LIKE 'PT50[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'");
                });

            // --- Comprador ---
            builder.Entity<Comprador>().ToTable("Comprador");

            // --- ConfNotificacoes (1-para-1) ---
            builder.Entity<ConfNotificacoes>().ToTable("ConfNotificacoes");

            // --- Morada (1-para-1) ---
            builder.Entity<Morada>()
                .ToTable("Morada", t => {
                    t.HasCheckConstraint("CK_Morada_CodigoPostal", "codigo_postal LIKE '[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9]'");
                });

            builder.Entity<Morada>()
                .HasOne(m => m.user)
                .WithOne(u => u.Morada)
                .HasForeignKey<Morada>(m => m.IdUtilizador)
                .OnDelete(DeleteBehavior.Cascade);
                

            // =================================================================
            // 2. TABELAS DE "LOOKUP" (Catálogo)
            // =================================================================

            builder.Entity<Marca>().ToTable("Marca");
            builder.Entity<Marca>().HasIndex(m => m.Nome).IsUnique();

            builder.Entity<Caixa>().ToTable("Caixa");
            builder.Entity<Caixa>().HasIndex(c => c.Nome).IsUnique();

            builder.Entity<Combustivel>().ToTable("Combustivel");
            builder.Entity<Combustivel>().HasIndex(c => c.Nome).IsUnique();

            builder.Entity<Categoria>().ToTable("Categoria");
            builder.Entity<Categoria>().HasIndex(c => c.Nome).IsUnique();

            builder.Entity<Cor>().ToTable("Cor");
            builder.Entity<Cor>().HasIndex(c => c.Nome).IsUnique();

            builder.Entity<Modelo>().ToTable("Modelo");
            builder.Entity<Modelo>()
                .HasOne(m => m.Marca)
                .WithMany()
                .HasForeignKey(m => m.IdMarca)
                .OnDelete(DeleteBehavior.NoAction);

            // =================================================================
            // 3. TABELAS DE SUPORTE (Ligadas ao Utilizador)
            // =================================================================

            builder.Entity<Contacto>().ToTable("Contacto");
            builder.Entity<Contacto>()
                .HasOne(c => c.User)
                .WithMany(u => u.Contactos)
                .HasForeignKey(c => c.IdUtilizador)
                .OnDelete(DeleteBehavior.Cascade);

            // --- MarcaFavoritaComprador (Muitos-para-Muitos) ---
            builder.Entity<MarcaFavoritaComprador>().ToTable("MarcaFavoritaComprador");
            builder.Entity<MarcaFavoritaComprador>()
                .HasKey(mf => new { mf.IdComprador, mf.IdMarca });

            builder.Entity<MarcaFavoritaComprador>()
                .HasOne(mf => mf.Comprador)
                .WithMany()
                .HasForeignKey(mf => mf.IdComprador)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MarcaFavoritaComprador>()
                .HasOne(mf => mf.Marca)
                .WithMany()
                .HasForeignKey(mf => mf.IdMarca)
                .OnDelete(DeleteBehavior.Cascade);

            // --- FiltroGuardado ---
            builder.Entity<FiltroGuardado>().ToTable("FiltroGuardado");
            builder.Entity<FiltroGuardado>()
                .HasOne(f => f.User).WithMany().HasForeignKey(f => f.IdUtilizador).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<FiltroGuardado>()
                .HasOne(f => f.Marca).WithMany().HasForeignKey(f => f.IdMarca).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<FiltroGuardado>()
                .HasOne(f => f.Modelo).WithMany().HasForeignKey(f => f.IdModelo).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<FiltroGuardado>()
                .HasOne(f => f.Cor).WithMany().HasForeignKey(f => f.IdCor).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<FiltroGuardado>()
                .HasOne(f => f.Caixa).WithMany().HasForeignKey(f => f.IdCaixa).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<FiltroGuardado>()
                .HasOne(f => f.Combustivel).WithMany().HasForeignKey(f => f.IdCombustivel).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<FiltroGuardado>()
                .HasOne(f => f.Categoria).WithMany().HasForeignKey(f => f.IdCategoria).OnDelete(DeleteBehavior.SetNull);

            builder.Entity<FiltroGuardado>().Property(f => f.PrecoMinimo).HasColumnType("money");
            builder.Entity<FiltroGuardado>().Property(f => f.PrecoMaximo).HasColumnType("money");
            // =================================================================
            // 4. VEÍCULOS E ANÚNCIOS (O Coração da App)
            // =================================================================

            builder.Entity<Veiculo>().ToTable("Veiculo");

            builder.Entity<Veiculo>()
                .HasOne(v => v.Vendedor).WithMany().HasForeignKey(v => v.IdVendedor).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Veiculo>()
                .HasOne(v => v.Modelo).WithMany().HasForeignKey(v => v.IdModelo).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Veiculo>()
                .HasOne(v => v.Cor).WithMany().HasForeignKey(v => v.IdCor).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Veiculo>()
                .HasOne(v => v.Caixa).WithMany().HasForeignKey(v => v.IdCaixa).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Veiculo>()
                .HasOne(v => v.Combustivel).WithMany().HasForeignKey(v => v.IdCombustivel).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Veiculo>()
                .HasOne(v => v.Categoria).WithMany().HasForeignKey(v => v.IdCategoria).OnDelete(DeleteBehavior.NoAction);

            // --- Imagem ---
            builder.Entity<Imagem>().ToTable("Imagem");
            builder.Entity<Imagem>().HasIndex(i => i.CaminhoFicheiro).IsUnique();
            builder.Entity<Imagem>()
                .HasOne(i => i.Veiculo).WithMany().HasForeignKey(i => i.IdVeiculo).OnDelete(DeleteBehavior.Cascade);

            // --- Anuncio ---
            builder.Entity<Anuncio>()
                .HasOne(a => a.Vendedor).WithMany().HasForeignKey(a => a.IdVendedor)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Anuncio>()
                .HasOne(a => a.Veiculo).WithMany().HasForeignKey(a => a.IdVeiculo).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Anuncio>()
                .ToTable("Anuncio", t => {
                    t.HasCheckConstraint("CK_Anuncio_Preco", "preco > 0");
                    t.HasCheckConstraint("CK_Anuncio_Estado", "estado IN ('Ativo', 'Reservado', 'Pausado', 'Vendido', 'Arquivado')");
                });

            builder.Entity<Anuncio>().Property(a => a.Preco).HasColumnType("money");

            // =================================================================
            // 5. INTERAÇÕES (Visitas, Reservas, Compras)
            // =================================================================

            builder.Entity<Visita>().ToTable("Visita");
            builder.Entity<Visita>()
                .HasOne(v => v.Comprador).WithMany().HasForeignKey(v => v.IdComprador).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Visita>()
                .HasOne(v => v.Anuncio).WithMany().HasForeignKey(v => v.IdAnuncio).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Visita>()
                .ToTable("Visita", t => {
                    t.HasCheckConstraint("CK_Visita_Estado", "estado_visita IN ('Pendente', 'Aprovada', 'Rejeitada', 'Expirada', 'Cancelada', 'Concluida', 'Vendido')");
                });

            builder.Entity<Reserva>().ToTable("Reserva");
            builder.Entity<Reserva>()
                .HasOne(r => r.Comprador).WithMany().HasForeignKey(r => r.IdComprador).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Reserva>()
                .HasOne(r => r.Anuncio).WithMany().HasForeignKey(r => r.IdAnuncio).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Reserva>()
                .ToTable("Reserva", t => {
                    t.HasCheckConstraint("CK_Reserva_Estado", "estado_reserva IN ('Pendente', 'Aprovada', 'Rejeitada', 'Expirada', 'Cancelada', 'Concluida', 'Vendido')");
                });

            builder.Entity<Compra>().ToTable("Compra");
            builder.Entity<Compra>()
                .HasOne(c => c.Comprador).WithMany().HasForeignKey(c => c.IdComprador).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Compra>()
                .HasOne(c => c.Anuncio).WithMany().HasForeignKey(c => c.IdAnuncio).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Compra>()
                .ToTable("Compra", t => {
                    t.HasCheckConstraint("CK_Compra_Estado", "estado_pagamento IN ('Pago', 'Pendente', 'Cancelado')");
                });

            builder.Entity<Compra>().Property(c => c.ValorTotal).HasColumnType("money");

            // =================================================================
            // 6. LOGS E AUDITORIA
            // =================================================================

            builder.Entity<Bloqueio>().ToTable("Bloqueio");
            builder.Entity<Bloqueio>()
                .HasOne(b => b.UtilizadorAlvo).WithMany().HasForeignKey(b => b.IdUtilizadorAlvo).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Bloqueio>()
                .HasOne(b => b.Administrador).WithMany().HasForeignKey(b => b.IdAdministrador).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<HistoricoDeAcoes>().ToTable("HistoricoDeAcoes");
            builder.Entity<HistoricoDeAcoes>()
                .HasOne(h => h.AdministradorAutor).WithMany().HasForeignKey(h => h.IdAdministradorAutor).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
