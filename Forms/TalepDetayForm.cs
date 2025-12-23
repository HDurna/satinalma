using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using satinalma.Data;

namespace satinalma.Forms
{
    public class TalepDetayForm : Form
    {
        public TalepDetayForm(int talepId, string talepNo)
        {
            this.Text = $"Talep Detayı - {talepNo}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // 1) Başlık bilgilerini veritabanından çek
            DataTable dtBaslik = Veritabani.VeriGetir($"SELECT * FROM TalepBaslik WHERE Id={talepId}");

            string tarih = "",
                   birim = "",
                   durum = "",
                   tip = "",
                   sevk = "",
                   eden = "";

            if (dtBaslik.Rows.Count > 0)
            {
                DataRow row = dtBaslik.Rows[0];

                tarih = Convert.ToDateTime(row["TalepTarihi"]).ToShortDateString();
                sevk = Convert.ToDateTime(row["SevkTarihi"]).ToShortDateString();
                birim = row["TalepBirimi"].ToString();
                durum = row["Durum"].ToString();
                tip = row["Tip"].ToString();

                // Yeni alan
                eden = row["TalepEden"]?.ToString() ?? "";
            }

            // 2) Ana başlık
            Label lblBaslik = new Label
            {
                Text = $"{talepNo} Nolu Talep Detayı",
                Location = new Point(20, 10),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.DarkSlateBlue
            };
            this.Controls.Add(lblBaslik);

            // -------------------------
            // 3) Genel Bilgiler Bölümü
            // -------------------------
            GroupBox grpBilgi = new GroupBox
            {
                Text = "Genel Bilgiler",
                Location = new Point(20, 50),
                Size = new Size(740, 120),
                Font = new Font("Segoe UI", 10)
            };

            Label lblTarih = new Label { Text = $"Talep Tarihi: {tarih}", Location = new Point(20, 30), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            Label lblBirim = new Label { Text = $"İsteyen Birim: {birim}", Location = new Point(250, 30), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            Label lblTip = new Label { Text = $"Talep Tipi: {tip}", Location = new Point(500, 30), AutoSize = true };

            Label lblSevk = new Label { Text = $"Sevk Tarihi: {sevk}", Location = new Point(20, 60), AutoSize = true };
            Label lblDurum = new Label { Text = $"Durum: {durum}", Location = new Point(250, 60), AutoSize = true, ForeColor = Color.Red };

            // YENİ – Talep Eden gösterimi
            Label lblEden = new Label
            {
                Text = $"Talep Eden: {eden}",
                Location = new Point(500, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            grpBilgi.Controls.Add(lblTarih);
            grpBilgi.Controls.Add(lblBirim);
            grpBilgi.Controls.Add(lblTip);
            grpBilgi.Controls.Add(lblSevk);
            grpBilgi.Controls.Add(lblDurum);
            grpBilgi.Controls.Add(lblEden);

            this.Controls.Add(grpBilgi);

            // -------------------------
            // 4) Ürün Listesi Başlığı
            // -------------------------
            Label lblUrunler = new Label
            {
                Text = "Talep Edilen Ürünler",
                Location = new Point(20, 180),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Underline)
            };
            this.Controls.Add(lblUrunler);

            // -------------------------
            // 5) Ürün Listesi (DataGrid)
            // -------------------------
            DataGridView grid = new DataGridView
            {
                Location = new Point(20, 210),
                Size = new Size(740, 330),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(grid);

            // Verileri yükle
            DataTable dtUrunler = Veritabani.VeriGetir($"SELECT UrunAdi, Miktar, Birim FROM TalepDetay WHERE TalepBaslikId={talepId}");
            grid.DataSource = dtUrunler;

            // Eğer ürün yoksa uyarı
            if (dtUrunler.Rows.Count == 0)
            {
                Label lblUyari = new Label
                {
                    Text = "Bu talebe ait ürün kaydı bulunamadı!",
                    Location = new Point(250, 260),
                    AutoSize = true,
                    ForeColor = Color.Red,
                    Font = new Font("Segoe UI", 12)
                };
                this.Controls.Add(lblUyari);
            }
        }
    }
}
