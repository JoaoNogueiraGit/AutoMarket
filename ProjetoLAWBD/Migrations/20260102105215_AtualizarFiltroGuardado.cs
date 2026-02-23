using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoLAWBD.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarFiltroGuardado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "latitude",
                table: "FiltroGuardado",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "localizacao",
                table: "FiltroGuardado",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                table: "FiltroGuardado",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "raio",
                table: "FiltroGuardado",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "termo_pesquisa",
                table: "FiltroGuardado",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude",
                table: "FiltroGuardado");

            migrationBuilder.DropColumn(
                name: "localizacao",
                table: "FiltroGuardado");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "FiltroGuardado");

            migrationBuilder.DropColumn(
                name: "raio",
                table: "FiltroGuardado");

            migrationBuilder.DropColumn(
                name: "termo_pesquisa",
                table: "FiltroGuardado");
        }
    }
}
