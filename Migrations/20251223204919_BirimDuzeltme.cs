using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace satinalma.Migrations
{
    /// <inheritdoc />
    public partial class BirimDuzeltme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HarcamaYetkilisi",
                table: "Birimler");

            migrationBuilder.AlterColumn<string>(
                name: "BirimAdi",
                table: "Birimler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BirimAdi",
                table: "Birimler",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "HarcamaYetkilisi",
                table: "Birimler",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
