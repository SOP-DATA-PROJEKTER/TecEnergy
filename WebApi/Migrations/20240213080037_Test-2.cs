using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MeterId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MeterId",
                table: "EnergyData");

            migrationBuilder.DropColumn(
                name: "MeterId",
                table: "DailyAccumulations");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "EnergyData",
                newName: "DateTime");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "DailyAccumulations",
                newName: "DateTime");

            migrationBuilder.AddColumn<bool>(
                name: "IsConnected",
                table: "EnergyMeters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "EnergyMeters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConnected",
                table: "EnergyMeters");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "EnergyMeters");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "EnergyData",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "DailyAccumulations",
                newName: "Date");

            migrationBuilder.AddColumn<Guid>(
                name: "BuildingId",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "MeterId",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MeterId",
                table: "EnergyData",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "MeterId",
                table: "DailyAccumulations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms",
                column: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
