using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateddatamodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnergyMetersId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "EnergyDatasId",
                table: "EnergyMeters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnergyMetersId",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnergyDatasId",
                table: "EnergyMeters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
