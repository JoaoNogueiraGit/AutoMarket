using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoLAWBD.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoVendido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Visita_Estado",
                table: "Visita");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reserva_Estado",
                table: "Reserva");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Visita_Estado",
                table: "Visita",
                sql: "estado_visita IN ('Pendente', 'Aprovada', 'Rejeitada', 'Expirada', 'Cancelada', 'Concluida', 'Vendido')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reserva_Estado",
                table: "Reserva",
                sql: "estado_reserva IN ('Pendente', 'Aprovada', 'Rejeitada', 'Expirada', 'Cancelada', 'Concluida', 'Vendido')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Visita_Estado",
                table: "Visita");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reserva_Estado",
                table: "Reserva");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Visita_Estado",
                table: "Visita",
                sql: "estado_visita IN ('Pendente', 'Aprovada', 'Rejeitada', 'Expirada', 'Cancelada', 'Concluida')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reserva_Estado",
                table: "Reserva",
                sql: "estado_reserva IN ('Pendente', 'Aprovada', 'Rejeitada', 'Expirada', 'Cancelada', 'Concluida')");
        }
    }
}
