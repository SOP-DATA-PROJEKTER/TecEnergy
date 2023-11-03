using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TecEnergy.Database.Migrations
{
    /// <inheritdoc />
    public partial class newModelRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingID",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_BuildingID",
                table: "Rooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingID",
                table: "Rooms",
                column: "BuildingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingID",
                table: "Rooms",
                column: "BuildingID",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
