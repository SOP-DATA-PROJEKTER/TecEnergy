using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TecEnergy.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRoomIdEnergyMeter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "EnergyMeters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoomID",
                table: "EnergyMeters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
