using System;
using System.Data;
using System.IO;
using System.Data.SQLite;

namespace satinalma.Data
{
    public static class Veritabani
    {
        private static string _dbDosyaAdi = "satinalma.db";
        private static string _baglantiCumlesi = $"Data Source={_dbDosyaAdi};Version=3;";

        public static void Baslat()
        {
            // Dosya yoksa oluştur
            if (!File.Exists(_dbDosyaAdi))
            {
                SQLiteConnection.CreateFile(_dbDosyaAdi);
            }

            using (var baglanti = new SQLiteConnection(_baglantiCumlesi))
            {
                baglanti.Open();
                using (var komut = new SQLiteCommand(baglanti))
                {
                    // ---------------------
                    // BİRİMLER TABLOSU
                    // ---------------------
                    komut.CommandText =
                        "CREATE TABLE IF NOT EXISTS Birimler (Id INTEGER PRIMARY KEY AUTOINCREMENT, BirimAdi TEXT, HarcamaYetkilisi TEXT)";
                    komut.ExecuteNonQuery();

                    // ---------------------
                    // KULLANICILAR TABLOSU
                    // ---------------------
                    komut.CommandText =
                        "CREATE TABLE IF NOT EXISTS Kullanicilar (Id INTEGER PRIMARY KEY AUTOINCREMENT, AdSoyad TEXT, KullaniciAdi TEXT, Sifre TEXT, Yetki TEXT)";
                    komut.ExecuteNonQuery();

                    // Admin yoksa ekle
                    komut.CommandText = "SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi='admin'";
                    var adminSayisi = Convert.ToInt32(komut.ExecuteScalar() ?? 0);
                    if (adminSayisi == 0)
                    {
                        komut.CommandText =
                            "INSERT INTO Kullanicilar (AdSoyad, KullaniciAdi, Sifre, Yetki) VALUES ('Yönetici', 'admin', '123', 'Admin')";
                        komut.ExecuteNonQuery();
                    }

                    // ---------------------
                    // KATEGORİLER TABLOSU
                    // ---------------------
                    komut.CommandText =
                        "CREATE TABLE IF NOT EXISTS Kategoriler (Id INTEGER PRIMARY KEY AUTOINCREMENT, KategoriAdi TEXT)";
                    komut.ExecuteNonQuery();

                    // ---------------------
                    // ÜRÜNLER TABLOSU
                    // ---------------------
                    komut.CommandText = @"
CREATE TABLE IF NOT EXISTS Urunler (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    KategoriAdi TEXT,
    UrunAdi TEXT,
    Ozellik1 TEXT,
    Ozellik2 TEXT,
    Ozellik3 TEXT,
    TamAd TEXT
)";
                    komut.ExecuteNonQuery();

                    // ---------------------
                    // ÖLÇÜ BİRİMLERİ
                    // ---------------------
                    komut.CommandText =
                        "CREATE TABLE IF NOT EXISTS OlcuBirimleri (Id INTEGER PRIMARY KEY AUTOINCREMENT, BirimAdi TEXT)";
                    komut.ExecuteNonQuery();

                    // ---------------------
                    // TALEP BAŞLIK TABLOSU
                    // (TalepEden ALANI EKLENDİ)
                    // ---------------------
                    komut.CommandText = @"
CREATE TABLE IF NOT EXISTS TalepBaslik (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TalepNo TEXT,
    TalepTarihi DATE,
    SevkTarihi DATE,
    TalepBirimi TEXT,
    Tip TEXT,
    EkapYapildi INTEGER,
    Durum TEXT,
    TalepEden TEXT,
    SilmeDurumu TEXT DEFAULT 'Yok',
    SilmeAciklamasi TEXT DEFAULT ''
)";
                    komut.ExecuteNonQuery();

                    // Var olan DB'lerde TalepEden yoksa ekle
                    komut.CommandText = "ALTER TABLE TalepBaslik ADD COLUMN TalepEden TEXT";
                    try { komut.ExecuteNonQuery(); } catch { }

                    // ---------------------
                    // TALEP DETAY TABLOSU
                    // ---------------------
                    komut.CommandText = @"
CREATE TABLE IF NOT EXISTS TalepDetay (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TalepBaslikId INTEGER,
    UrunId INTEGER,
    UrunAdi TEXT,
    Miktar REAL,
    Birim TEXT
)";
                    komut.ExecuteNonQuery();

                    // ---------------------
                    // SİLME LOG
                    // ---------------------
                    komut.CommandText = @"
CREATE TABLE IF NOT EXISTS SilmeLog (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TalepBaslikId INTEGER,
    TalepNo TEXT,
    IslemTarihi TEXT,
    IslemTipi TEXT,
    KullaniciAdi TEXT,
    Aciklama TEXT
)";
                    komut.ExecuteNonQuery();

                    // ---------------------
                    // BİLDİRİMLER
                    // ---------------------
                    komut.CommandText = @"
CREATE TABLE IF NOT EXISTS Bildirimler (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    KullaniciAdi TEXT,
    Mesaj TEXT,
    Tarih TEXT,
    Okundu INTEGER DEFAULT 0
)";
                    komut.ExecuteNonQuery();
                }
            }
        }

        public static DataTable VeriGetir(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var baglanti = new SQLiteConnection(_baglantiCumlesi))
                {
                    baglanti.Open();
                    using (var adaptor = new SQLiteDataAdapter(sql, baglanti))
                    {
                        adaptor.Fill(dt);
                    }
                }
            }
            catch { }
            return dt;
        }

        public static void SorguCalistir(string sql)
        {
            try
            {
                using (var baglanti = new SQLiteConnection(_baglantiCumlesi))
                {
                    baglanti.Open();
                    using (var komut = new SQLiteCommand(sql, baglanti))
                    {
                        komut.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("İşlem Hatası: " + ex.Message);
            }
        }

        public static object TekDegerGetir(string sql)
        {
            try
            {
                using (var baglanti = new SQLiteConnection(_baglantiCumlesi))
                {
                    baglanti.Open();
                    using (var komut = new SQLiteCommand(sql, baglanti))
                    {
                        return komut.ExecuteScalar();
                    }
                }
            }
            catch { return null; }
        }

        public static string YeniTalepNoUret(string birimAdi)
        {
            string onEk = birimAdi.Length >= 3 ? birimAdi.Substring(0, 3).ToUpper() : "GEN";
            object sonuc = TekDegerGetir("SELECT MAX(Id) FROM TalepBaslik");
            int sayi = sonuc == null || sonuc == DBNull.Value ? 1 : Convert.ToInt32(sonuc) + 1;
            return $"{onEk}-{DateTime.Now:yyyyMMdd}-{sayi}";
        }
    }
}
