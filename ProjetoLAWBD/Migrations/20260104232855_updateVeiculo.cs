using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoLAWBD.Migrations
{
    /// <inheritdoc />
    public partial class updateVeiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Arquivado",
                table: "Veiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arquivado",
                table: "Veiculo");
        }
    }
}
