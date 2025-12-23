using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace satinalma.Migrations
{
    /// <inheritdoc />
    public partial class IlkKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Okundu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Birimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BirimAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HarcamaYetkilisi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Birimler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategoriAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Yetki = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OlcuBirimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BirimAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OlcuBirimleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SilmeLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TalepBaslikId = table.Column<int>(type: "int", nullable: false),
                    TalepNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IslemTipi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SilmeLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TalepBaslik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TalepNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TalepTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SevkTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TalepBirimi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EkapYapildi = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TalepEden = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SilmeDurumu = table.Column<int>(type: "int", nullable: false),
                    SilmeAciklamasi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SiparisId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalepBaslik", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TedarikciFirmalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirmaAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FirmaTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VergiNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    YetkiliKisi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TedarikciFirmalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategoriAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UrunAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ozellik1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ozellik2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ozellik3 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TamAd = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TalepDetay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TalepBaslikId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    UrunAdi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Miktar = table.Column<double>(type: "float", nullable: false),
                    Birim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiparisMiktari = table.Column<double>(type: "float", nullable: false),
                    KalanMiktar = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalepDetay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TalepDetay_TalepBaslik_TalepBaslikId",
                        column: x => x.TalepBaslikId,
                        principalTable: "TalepBaslik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiparisBaslik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiparisNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiparisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TedarikciFirmaId = table.Column<int>(type: "int", nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturanKullanici = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EkNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisBaslik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisBaslik_TedarikciFirmalar_TedarikciFirmaId",
                        column: x => x.TedarikciFirmaId,
                        principalTable: "TedarikciFirmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiparisDetay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiparisBaslikId = table.Column<int>(type: "int", nullable: false),
                    TalepDetayId = table.Column<int>(type: "int", nullable: true),
                    UrunAdi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Miktar = table.Column<double>(type: "float", nullable: false),
                    Birim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TeslimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisDetay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisDetay_SiparisBaslik_SiparisBaslikId",
                        column: x => x.SiparisBaslikId,
                        principalTable: "SiparisBaslik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Kullanicilar",
                columns: new[] { "Id", "AdSoyad", "KullaniciAdi", "Sifre", "Yetki" },
                values: new object[] { 1, "Admin User", "admin", "123", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiparisBaslik_SiparisNo",
                table: "SiparisBaslik",
                column: "SiparisNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiparisBaslik_TedarikciFirmaId",
                table: "SiparisBaslik",
                column: "TedarikciFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDetay_SiparisBaslikId",
                table: "SiparisDetay",
                column: "SiparisBaslikId");

            migrationBuilder.CreateIndex(
                name: "IX_TalepBaslik_TalepNo",
                table: "TalepBaslik",
                column: "TalepNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TalepDetay_TalepBaslikId",
                table: "TalepDetay",
                column: "TalepBaslikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "Birimler");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "OlcuBirimleri");

            migrationBuilder.DropTable(
                name: "SilmeLog");

            migrationBuilder.DropTable(
                name: "SiparisDetay");

            migrationBuilder.DropTable(
                name: "TalepDetay");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "SiparisBaslik");

            migrationBuilder.DropTable(
                name: "TalepBaslik");

            migrationBuilder.DropTable(
                name: "TedarikciFirmalar");
        }
    }
}
