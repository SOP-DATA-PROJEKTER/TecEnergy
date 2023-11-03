using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TecEnergy.Database.Migrations
{
    /// <inheritdoc />
    public partial class energymetermodelremovedonlyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnergyData_EnergyMeters_EnergyMeterID",
                table: "EnergyData");

            migrationBuilder.DropIndex(
                name: "IX_EnergyData_EnergyMeterID",
                table: "EnergyData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EnergyData_EnergyMeterID",
                table: "EnergyData",
                column: "EnergyMeterID");

            migrationBuilder.AddForeignKey(
                name: "FK_EnergyData_EnergyMeters_EnergyMeterID",
                table: "EnergyData",
                column: "EnergyMeterID",
                principalTable: "EnergyMeters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
