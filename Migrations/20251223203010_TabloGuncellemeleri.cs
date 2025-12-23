using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace satinalma.Migrations
{
    /// <inheritdoc />
    public partial class TabloGuncellemeleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TalepDetay_TalepBaslik_TalepBaslikId",
                table: "TalepDetay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TalepBaslik",
                table: "TalepBaslik");

            migrationBuilder.DropColumn(
                name: "KategoriAdi",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "Ozellik1",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "Ozellik2",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "Ozellik3",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "TamAd",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "SevkTarihi",
                table: "TalepBaslik");

            migrationBuilder.DropColumn(
                name: "SilmeAciklamasi",
                table: "TalepBaslik");

            migrationBuilder.DropColumn(
                name: "SiparisId",
                table: "TalepBaslik");

            migrationBuilder.DropColumn(
                name: "TalepBirimi",
                table: "TalepBaslik");

            migrationBuilder.DropColumn(
                name: "TalepEden",
                table: "TalepBaslik");

            migrationBuilder.RenameTable(
                name: "TalepBaslik",
                newName: "TalepBasliklar");

            migrationBuilder.RenameColumn(
                name: "TalepTarihi",
                table: "TalepBasliklar",
                newName: "Tarih");

            migrationBuilder.RenameColumn(
                name: "SilmeDurumu",
                table: "TalepBasliklar",
                newName: "KullaniciId");

            migrationBuilder.RenameIndex(
                name: "IX_TalepBaslik_TalepNo",
                table: "TalepBasliklar",
                newName: "IX_TalepBasliklar_TalepNo");

            migrationBuilder.AlterColumn<string>(
                name: "UrunAdi",
                table: "Urunler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "Urunler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Aktif",
                table: "Urunler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BirimId",
                table: "Urunler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KategoriId",
                table: "Urunler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TalepNo",
                table: "TalepBasliklar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "EkapYapildi",
                table: "TalepBasliklar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Aciklama",
                table: "TalepBasliklar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TalepBasliklar",
                table: "TalepBasliklar",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_BirimId",
                table: "Urunler",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_KategoriId",
                table: "Urunler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_TalepBasliklar_KullaniciId",
                table: "TalepBasliklar",
                column: "KullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TalepBasliklar_Kullanicilar_KullaniciId",
                table: "TalepBasliklar",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TalepDetay_TalepBasliklar_TalepBaslikId",
                table: "TalepDetay",
                column: "TalepBaslikId",
                principalTable: "TalepBasliklar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Urunler_Birimler_BirimId",
                table: "Urunler",
                column: "BirimId",
                principalTable: "Birimler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Urunler_Kategoriler_KategoriId",
                table: "Urunler",
                column: "KategoriId",
                principalTable: "Kategoriler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TalepBasliklar_Kullanicilar_KullaniciId",
                table: "TalepBasliklar");

            migrationBuilder.DropForeignKey(
                name: "FK_TalepDetay_TalepBasliklar_TalepBaslikId",
                table: "TalepDetay");

            migrationBuilder.DropForeignKey(
                name: "FK_Urunler_Birimler_BirimId",
                table: "Urunler");

            migrationBuilder.DropForeignKey(
                name: "FK_Urunler_Kategoriler_KategoriId",
                table: "Urunler");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_BirimId",
                table: "Urunler");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_KategoriId",
                table: "Urunler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TalepBasliklar",
                table: "TalepBasliklar");

            migrationBuilder.DropIndex(
                name: "IX_TalepBasliklar_KullaniciId",
                table: "TalepBasliklar");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "Aktif",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "BirimId",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "KategoriId",
                table: "Urunler");

            migrationBuilder.RenameTable(
                name: "TalepBasliklar",
                newName: "TalepBaslik");

            migrationBuilder.RenameColumn(
                name: "Tarih",
                table: "TalepBaslik",
                newName: "TalepTarihi");

            migrationBuilder.RenameColumn(
                name: "KullaniciId",
                table: "TalepBaslik",
                newName: "SilmeDurumu");

            migrationBuilder.RenameIndex(
                name: "IX_TalepBasliklar_TalepNo",
                table: "TalepBaslik",
                newName: "IX_TalepBaslik_TalepNo");

            migrationBuilder.AlterColumn<string>(
                name: "UrunAdi",
                table: "Urunler",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "KategoriAdi",
                table: "Urunler",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ozellik1",
                table: "Urunler",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ozellik2",
                table: "Urunler",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ozellik3",
                table: "Urunler",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TamAd",
                table: "Urunler",
                type: "nvarchar(800)",
                maxLength: 800,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TalepNo",
                table: "TalepBaslik",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "EkapYapildi",
                table: "TalepBaslik",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Aciklama",
                table: "TalepBaslik",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "SevkTarihi",
                table: "TalepBaslik",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilmeAciklamasi",
                table: "TalepBaslik",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SiparisId",
                table: "TalepBaslik",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TalepBirimi",
                table: "TalepBaslik",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TalepEden",
                table: "TalepBaslik",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TalepBaslik",
                table: "TalepBaslik",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TalepDetay_TalepBaslik_TalepBaslikId",
                table: "TalepDetay",
                column: "TalepBaslikId",
                principalTable: "TalepBaslik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
