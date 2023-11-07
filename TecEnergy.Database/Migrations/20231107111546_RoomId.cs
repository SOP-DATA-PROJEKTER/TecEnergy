using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TecEnergy.Database.Migrations
{
    /// <inheritdoc />
    public partial class RoomId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnergyMeters_Rooms_RoomId",
                table: "EnergyMeters");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "EnergyMeters",
                newName: "RoomID");

            migrationBuilder.RenameIndex(
                name: "IX_EnergyMeters_RoomId",
                table: "EnergyMeters",
                newName: "IX_EnergyMeters_RoomID");

            migrationBuilder.AddForeignKey(
                name: "FK_EnergyMeters_Rooms_RoomID",
                table: "EnergyMeters",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnergyMeters_Rooms_RoomID",
                table: "EnergyMeters");

            migrationBuilder.RenameColumn(
                name: "RoomID",
                table: "EnergyMeters",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_EnergyMeters_RoomID",
                table: "EnergyMeters",
                newName: "IX_EnergyMeters_RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnergyMeters_Rooms_RoomId",
                table: "EnergyMeters",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }
    }
}
