using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoLAWBD.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompraDadosFaturacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CPFaturacao",
                table: "Compra",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CidadeFaturacao",
                table: "Compra",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                table: "Compra",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NIFVendedor",
                table: "Compra",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeFaturacao",
                table: "Compra",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RuaFaturacao",
                table: "Compra",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPFaturacao",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "CidadeFaturacao",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "IBAN",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "NIFVendedor",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "NomeFaturacao",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "RuaFaturacao",
                table: "Compra");
        }
    }
}
