using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TecEnergy.Database.Migrations
{
    /// <inheritdoc />
    public partial class DailyAccumulatedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasurementType",
                table: "EnergyMeters");

            migrationBuilder.DropColumn(
                name: "ReadingFrequency",
                table: "EnergyMeters");

            migrationBuilder.CreateTable(
                name: "DailyAccumulated",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DailyAccumulatedValue = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyAccumulated", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyAccumulated_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyAccumulated_DateTime",
                table: "DailyAccumulated",
                column: "DateTime",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyAccumulated_RoomId",
                table: "DailyAccumulated",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyAccumulated");

            migrationBuilder.AddColumn<string>(
                name: "MeasurementType",
                table: "EnergyMeters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadingFrequency",
                table: "EnergyMeters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
