using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoLAWBD.Migrations
{
    /// <inheritdoc />
    public partial class Reset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Caixa",
                columns: table => new
                {
                    id_caixa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caixa", x => x.id_caixa);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "Combustivel",
                columns: table => new
                {
                    id_combustivel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combustivel", x => x.id_combustivel);
                });

            migrationBuilder.CreateTable(
                name: "Cor",
                columns: table => new
                {
                    id_cor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cor", x => x.id_cor);
                });

            migrationBuilder.CreateTable(
                name: "Marca",
                columns: table => new
                {
                    id_marca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marca", x => x.id_marca);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    id_utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_administrador_criador = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrador", x => x.id_utilizador);
                    table.ForeignKey(
                        name: "FK_Administrador_Administrador_id_administrador_criador",
                        column: x => x.id_administrador_criador,
                        principalTable: "Administrador",
                        principalColumn: "id_utilizador");
                    table.ForeignKey(
                        name: "FK_Administrador_AspNetUsers_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comprador",
                columns: table => new
                {
                    id_utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprador", x => x.id_utilizador);
                    table.ForeignKey(
                        name: "FK_Comprador_AspNetUsers_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfNotificacoes",
                columns: table => new
                {
                    id_utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    anuncios_marcas_favoritas = table.Column<bool>(type: "bit", nullable: false),
                    alteracoes_reservas_visitas = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfNotificacoes", x => x.id_utilizador);
                    table.ForeignKey(
                        name: "FK_ConfNotificacoes_AspNetUsers_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacto",
                columns: table => new
                {
                    id_contacto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    tipo_contacto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contacto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacto", x => x.id_contacto);
                    table.ForeignKey(
                        name: "FK_Contacto_AspNetUsers_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Morada",
                columns: table => new
                {
                    id_utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    rua_e_numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    codigo_postal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    distrito = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cidade = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Morada", x => x.id_utilizador);
                    table.CheckConstraint("CK_Morada_CodigoPostal", "codigo_postal LIKE '[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9]'");
                    table.ForeignKey(
                        name: "FK_Morada_AspNetUsers_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modelo",
                columns: table => new
                {
                    id_modelo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_marca = table.Column<int>(type: "int", nullable: false),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelo", x => x.id_modelo);
                    table.ForeignKey(
                        name: "FK_Modelo_Marca_id_marca",
                        column: x => x.id_marca,
                        principalTable: "Marca",
                        principalColumn: "id_marca");
                });

            migrationBuilder.CreateTable(
                name: "Bloqueio",
                columns: table => new
                {
                    id_bloqueio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_utilizador_alvo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_administrador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    data_bloqueio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    motivo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bloqueio", x => x.id_bloqueio);
                    table.ForeignKey(
                        name: "FK_Bloqueio_Administrador_id_administrador",
                        column: x => x.id_administrador,
                        principalTable: "Administrador",
                        principalColumn: "id_utilizador");
                    table.ForeignKey(
                        name: "FK_Bloqueio_AspNetUsers_id_utilizador_alvo",
                        column: x => x.id_utilizador_alvo,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoDeAcoes",
                columns: table => new
                {
                    id_acao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_administrador_autor = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    data_acao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tipo_acao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    entidade_alvo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_entidade_alvo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoDeAcoes", x => x.id_acao);
                    table.ForeignKey(
                        name: "FK_HistoricoDeAcoes_Administrador_id_administrador_autor",
                        column: x => x.id_administrador_autor,
                        principalTable: "Administrador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Vendedor",
                columns: table => new
                {
                    id_utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    tipo_vendedor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nif = table.Column<string>(type: "varchar(9)", nullable: false),
                    id_administrador_validador = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    estado_validacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_validacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    nome_faturacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    morada_faturacao_rua = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    morada_faturacao_codigo_postal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    morada_faturacao_cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    iban = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedor", x => x.id_utilizador);
                    table.CheckConstraint("CK_Vendedor_CodigoPostal", "morada_faturacao_codigo_postal IS NULL OR morada_faturacao_codigo_postal LIKE '[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9]'");
                    table.CheckConstraint("CK_Vendedor_Estado", "estado_validacao IN ('Pendente', 'Aprovado', 'Rejeitado')");
                    table.CheckConstraint("CK_Vendedor_IBAN", "iban IS NULL OR iban LIKE 'PT50[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'");
                    table.CheckConstraint("CK_Vendedor_NIF", "nif LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'");
                    table.CheckConstraint("CK_Vendedor_Tipo", "tipo_vendedor IN ('Particular', 'Empresa')");
                    table.ForeignKey(
                        name: "FK_Vendedor_Administrador_id_administrador_validador",
                        column: x => x.id_administrador_validador,
                        principalTable: "Administrador",
                        principalColumn: "id_utilizador");
                    table.ForeignKey(
                        name: "FK_Vendedor_AspNetUsers_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarcaFavoritaComprador",
                columns: table => new
                {
                    id_comprador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_marca = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarcaFavoritaComprador", x => new { x.id_comprador, x.id_marca });
                    table.ForeignKey(
                        name: "FK_MarcaFavoritaComprador_Comprador_id_comprador",
                        column: x => x.id_comprador,
                        principalTable: "Comprador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarcaFavoritaComprador_Marca_id_marca",
                        column: x => x.id_marca,
                        principalTable: "Marca",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FiltroGuardado",
                columns: table => new
                {
                    id_filtro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nome_filtro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_marca = table.Column<int>(type: "int", nullable: true),
                    id_modelo = table.Column<int>(type: "int", nullable: true),
                    id_cor = table.Column<int>(type: "int", nullable: true),
                    id_caixa = table.Column<int>(type: "int", nullable: true),
                    id_combustivel = table.Column<int>(type: "int", nullable: true),
                    id_categoria = table.Column<int>(type: "int", nullable: true),
                    ano_minimo = table.Column<int>(type: "int", nullable: true),
                    ano_maximo = table.Column<int>(type: "int", nullable: true),
                    km_maximo = table.Column<int>(type: "int", nullable: true),
                    preco_minimo = table.Column<decimal>(type: "money", nullable: true),
                    preco_maximo = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltroGuardado", x => x.id_filtro);
                    table.ForeignKey(
                        name: "FK_FiltroGuardado_AspNetUsers_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FiltroGuardado_Caixa_id_caixa",
                        column: x => x.id_caixa,
                        principalTable: "Caixa",
                        principalColumn: "id_caixa",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FiltroGuardado_Categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "Categoria",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FiltroGuardado_Combustivel_id_combustivel",
                        column: x => x.id_combustivel,
                        principalTable: "Combustivel",
                        principalColumn: "id_combustivel",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FiltroGuardado_Cor_id_cor",
                        column: x => x.id_cor,
                        principalTable: "Cor",
                        principalColumn: "id_cor",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FiltroGuardado_Marca_id_marca",
                        column: x => x.id_marca,
                        principalTable: "Marca",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FiltroGuardado_Modelo_id_modelo",
                        column: x => x.id_modelo,
                        principalTable: "Modelo",
                        principalColumn: "id_modelo",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Veiculo",
                columns: table => new
                {
                    id_veiculo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_vendedor = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_modelo = table.Column<int>(type: "int", nullable: false),
                    id_cor = table.Column<int>(type: "int", nullable: false),
                    id_caixa = table.Column<int>(type: "int", nullable: false),
                    id_combustivel = table.Column<int>(type: "int", nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    potencia_cv = table.Column<int>(type: "int", nullable: false),
                    cilindrada_cc = table.Column<int>(type: "int", nullable: true),
                    num_lugares = table.Column<int>(type: "int", nullable: false),
                    num_portas = table.Column<int>(type: "int", nullable: false),
                    data_matricula = table.Column<DateTime>(type: "datetime2", nullable: false),
                    km_total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculo", x => x.id_veiculo);
                    table.ForeignKey(
                        name: "FK_Veiculo_Caixa_id_caixa",
                        column: x => x.id_caixa,
                        principalTable: "Caixa",
                        principalColumn: "id_caixa");
                    table.ForeignKey(
                        name: "FK_Veiculo_Categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "Categoria",
                        principalColumn: "id_categoria");
                    table.ForeignKey(
                        name: "FK_Veiculo_Combustivel_id_combustivel",
                        column: x => x.id_combustivel,
                        principalTable: "Combustivel",
                        principalColumn: "id_combustivel");
                    table.ForeignKey(
                        name: "FK_Veiculo_Cor_id_cor",
                        column: x => x.id_cor,
                        principalTable: "Cor",
                        principalColumn: "id_cor");
                    table.ForeignKey(
                        name: "FK_Veiculo_Modelo_id_modelo",
                        column: x => x.id_modelo,
                        principalTable: "Modelo",
                        principalColumn: "id_modelo");
                    table.ForeignKey(
                        name: "FK_Veiculo_Vendedor_id_vendedor",
                        column: x => x.id_vendedor,
                        principalTable: "Vendedor",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Anuncio",
                columns: table => new
                {
                    id_anuncio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_vendedor = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_veiculo = table.Column<int>(type: "int", nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    preco = table.Column<decimal>(type: "money", nullable: false),
                    localizacao_cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_publicacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anuncio", x => x.id_anuncio);
                    table.CheckConstraint("CK_Anuncio_Estado", "estado IN ('Ativo', 'Reservado', 'Pausado', 'Vendido', 'Arquivado')");
                    table.CheckConstraint("CK_Anuncio_Preco", "preco > 0");
                    table.ForeignKey(
                        name: "FK_Anuncio_Veiculo_id_veiculo",
                        column: x => x.id_veiculo,
                        principalTable: "Veiculo",
                        principalColumn: "id_veiculo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Anuncio_Vendedor_id_vendedor",
                        column: x => x.id_vendedor,
                        principalTable: "Vendedor",
                        principalColumn: "id_utilizador");
                });

            migrationBuilder.CreateTable(
                name: "Imagem",
                columns: table => new
                {
                    id_imagem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_veiculo = table.Column<int>(type: "int", nullable: false),
                    caminho_ficheiro = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    is_capa = table.Column<bool>(type: "bit", nullable: false),
                    ordem_exibicao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagem", x => x.id_imagem);
                    table.ForeignKey(
                        name: "FK_Imagem_Veiculo_id_veiculo",
                        column: x => x.id_veiculo,
                        principalTable: "Veiculo",
                        principalColumn: "id_veiculo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compra",
                columns: table => new
                {
                    id_compra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_comprador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_anuncio = table.Column<int>(type: "int", nullable: false),
                    data_compra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado_pagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valor_total = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compra", x => x.id_compra);
                    table.CheckConstraint("CK_Compra_Estado", "estado_pagamento IN ('Pago', 'Pendente', 'Cancelado')");
                    table.ForeignKey(
                        name: "FK_Compra_Anuncio_id_anuncio",
                        column: x => x.id_anuncio,
                        principalTable: "Anuncio",
                        principalColumn: "id_anuncio");
                    table.ForeignKey(
                        name: "FK_Compra_Comprador_id_comprador",
                        column: x => x.id_comprador,
                        principalTable: "Comprador",
                        principalColumn: "id_utilizador");
                });

            migrationBuilder.CreateTable(
                name: "Reserva",
                columns: table => new
                {
                    id_reserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_comprador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_anuncio = table.Column<int>(type: "int", nullable: false),
                    estado_reserva = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_pedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prazo_expiracao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserva", x => x.id_reserva);
                    table.CheckConstraint("CK_Reserva_Estado", "estado_reserva IN ('Pendente', 'Aprovada', 'Rejeitada', 'Expirada', 'Cancelada', 'Concluida')");
                    table.ForeignKey(
                        name: "FK_Reserva_Anuncio_id_anuncio",
                        column: x => x.id_anuncio,
                        principalTable: "Anuncio",
                        principalColumn: "id_anuncio");
                    table.ForeignKey(
                        name: "FK_Reserva_Comprador_id_comprador",
                        column: x => x.id_comprador,
                        principalTable: "Comprador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visita",
                columns: table => new
                {
                    id_visita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_comprador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_anuncio = table.Column<int>(type: "int", nullable: false),
                    estado_visita = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_pedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_visita = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visita", x => x.id_visita);
                    table.CheckConstraint("CK_Visita_Estado", "estado_visita IN ('Pendente', 'Aprovada', 'Rejeitada', 'Expirada', 'Cancelada', 'Concluida')");
                    table.ForeignKey(
                        name: "FK_Visita_Anuncio_id_anuncio",
                        column: x => x.id_anuncio,
                        principalTable: "Anuncio",
                        principalColumn: "id_anuncio");
                    table.ForeignKey(
                        name: "FK_Visita_Comprador_id_comprador",
                        column: x => x.id_comprador,
                        principalTable: "Comprador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrador_id_administrador_criador",
                table: "Administrador",
                column: "id_administrador_criador");

            migrationBuilder.CreateIndex(
                name: "IX_Anuncio_id_veiculo",
                table: "Anuncio",
                column: "id_veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_Anuncio_id_vendedor",
                table: "Anuncio",
                column: "id_vendedor");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bloqueio_id_administrador",
                table: "Bloqueio",
                column: "id_administrador");

            migrationBuilder.CreateIndex(
                name: "IX_Bloqueio_id_utilizador_alvo",
                table: "Bloqueio",
                column: "id_utilizador_alvo");

            migrationBuilder.CreateIndex(
                name: "IX_Caixa_nome",
                table: "Caixa",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_nome",
                table: "Categoria",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Combustivel_nome",
                table: "Combustivel",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compra_id_anuncio",
                table: "Compra",
                column: "id_anuncio");

            migrationBuilder.CreateIndex(
                name: "IX_Compra_id_comprador",
                table: "Compra",
                column: "id_comprador");

            migrationBuilder.CreateIndex(
                name: "IX_Contacto_id_utilizador",
                table: "Contacto",
                column: "id_utilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Cor_nome",
                table: "Cor",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FiltroGuardado_id_caixa",
                table: "FiltroGuardado",
                column: "id_caixa");

            migrationBuilder.CreateIndex(
                name: "IX_FiltroGuardado_id_categoria",
                table: "FiltroGuardado",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_FiltroGuardado_id_combustivel",
                table: "FiltroGuardado",
                column: "id_combustivel");

            migrationBuilder.CreateIndex(
                name: "IX_FiltroGuardado_id_cor",
                table: "FiltroGuardado",
                column: "id_cor");

            migrationBuilder.CreateIndex(
                name: "IX_FiltroGuardado_id_marca",
                table: "FiltroGuardado",
                column: "id_marca");

            migrationBuilder.CreateIndex(
                name: "IX_FiltroGuardado_id_modelo",
                table: "FiltroGuardado",
                column: "id_modelo");

            migrationBuilder.CreateIndex(
                name: "IX_FiltroGuardado_id_utilizador",
                table: "FiltroGuardado",
                column: "id_utilizador");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoDeAcoes_id_administrador_autor",
                table: "HistoricoDeAcoes",
                column: "id_administrador_autor");

            migrationBuilder.CreateIndex(
                name: "IX_Imagem_caminho_ficheiro",
                table: "Imagem",
                column: "caminho_ficheiro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imagem_id_veiculo",
                table: "Imagem",
                column: "id_veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_Marca_nome",
                table: "Marca",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarcaFavoritaComprador_id_marca",
                table: "MarcaFavoritaComprador",
                column: "id_marca");

            migrationBuilder.CreateIndex(
                name: "IX_Modelo_id_marca",
                table: "Modelo",
                column: "id_marca");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_id_anuncio",
                table: "Reserva",
                column: "id_anuncio");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_id_comprador",
                table: "Reserva",
                column: "id_comprador");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_id_caixa",
                table: "Veiculo",
                column: "id_caixa");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_id_categoria",
                table: "Veiculo",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_id_combustivel",
                table: "Veiculo",
                column: "id_combustivel");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_id_cor",
                table: "Veiculo",
                column: "id_cor");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_id_modelo",
                table: "Veiculo",
                column: "id_modelo");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_id_vendedor",
                table: "Veiculo",
                column: "id_vendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Vendedor_id_administrador_validador",
                table: "Vendedor",
                column: "id_administrador_validador");

            migrationBuilder.CreateIndex(
                name: "IX_Vendedor_nif",
                table: "Vendedor",
                column: "nif",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visita_id_anuncio",
                table: "Visita",
                column: "id_anuncio");

            migrationBuilder.CreateIndex(
                name: "IX_Visita_id_comprador",
                table: "Visita",
                column: "id_comprador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Bloqueio");

            migrationBuilder.DropTable(
                name: "Compra");

            migrationBuilder.DropTable(
                name: "ConfNotificacoes");

            migrationBuilder.DropTable(
                name: "Contacto");

            migrationBuilder.DropTable(
                name: "FiltroGuardado");

            migrationBuilder.DropTable(
                name: "HistoricoDeAcoes");

            migrationBuilder.DropTable(
                name: "Imagem");

            migrationBuilder.DropTable(
                name: "MarcaFavoritaComprador");

            migrationBuilder.DropTable(
                name: "Morada");

            migrationBuilder.DropTable(
                name: "Reserva");

            migrationBuilder.DropTable(
                name: "Visita");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Anuncio");

            migrationBuilder.DropTable(
                name: "Comprador");

            migrationBuilder.DropTable(
                name: "Veiculo");

            migrationBuilder.DropTable(
                name: "Caixa");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Combustivel");

            migrationBuilder.DropTable(
                name: "Cor");

            migrationBuilder.DropTable(
                name: "Modelo");

            migrationBuilder.DropTable(
                name: "Vendedor");

            migrationBuilder.DropTable(
                name: "Marca");

            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
