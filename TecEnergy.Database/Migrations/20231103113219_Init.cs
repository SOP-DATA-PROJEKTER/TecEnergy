using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TecEnergy.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnergyMeters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadingFrequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeasurementPointName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeasurementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstallmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeasurementPointComment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyMeters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnergyData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnergyMeterID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnergyData_EnergyMeters_EnergyMeterID",
                        column: x => x.EnergyMeterID,
                        principalTable: "EnergyMeters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnergyData_EnergyMeterID",
                table: "EnergyData",
                column: "EnergyMeterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnergyData");

            migrationBuilder.DropTable(
                name: "EnergyMeters");
        }
    }
}
