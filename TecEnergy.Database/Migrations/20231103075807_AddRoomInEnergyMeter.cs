using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TecEnergy.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomInEnergyMeter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoomID",
                table: "EnergyMeters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "EnergyMeters");
        }
    }
}
