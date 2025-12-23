using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace satinalma.Migrations
{
    /// <inheritdoc />
    public partial class EskiYapiyiGetirdik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tarih",
                table: "TalepBasliklar",
                newName: "TalepTarihi");

            migrationBuilder.AddColumn<DateTime>(
                name: "SevkTarihi",
                table: "TalepBasliklar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TalepBirimi",
                table: "TalepBasliklar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TalepEden",
                table: "TalepBasliklar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SevkTarihi",
                table: "TalepBasliklar");

            migrationBuilder.DropColumn(
                name: "TalepBirimi",
                table: "TalepBasliklar");

            migrationBuilder.DropColumn(
                name: "TalepEden",
                table: "TalepBasliklar");

            migrationBuilder.RenameColumn(
                name: "TalepTarihi",
                table: "TalepBasliklar",
                newName: "Tarih");
        }
    }
}
