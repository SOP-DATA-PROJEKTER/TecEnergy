using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updatedmodels2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnergyMeterId",
                table: "Rooms",
                newName: "EnergyMetersId");

            migrationBuilder.RenameColumn(
                name: "EnergyMeters",
                table: "EnergyMeters",
                newName: "EnergyDatasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnergyMetersId",
                table: "Rooms",
                newName: "EnergyMeterId");

            migrationBuilder.RenameColumn(
                name: "EnergyDatasId",
                table: "EnergyMeters",
                newName: "EnergyMeters");
        }
    }
}
