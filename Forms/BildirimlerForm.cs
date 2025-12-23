using System;
using System.Drawing;
using System.Windows.Forms;
using satinalma.Data;

namespace satinalma.Forms
{
    public class BildirimlerForm : Form
    {
        private readonly string _kullaniciAdi;
        private DataGridView _grid;

        public BildirimlerForm(string kullaniciAdi)
        {
            _kullaniciAdi = kullaniciAdi;

            // ORTAK TEMA
            FormGorunum.Uygula(this);

            this.Text = "Bildirimlerim";
            this.Size = new Size(700, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(_grid);

            Button btnOkundu = new Button
            {
                Text = "TÜMÜNÜ OKUNDU İŞARETLE",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White
            };
            btnOkundu.Click += (s, e) =>
            {
                Veritabani.SorguCalistir($"UPDATE Bildirimler SET Okundu=1 WHERE KullaniciAdi='{_kullaniciAdi}'");
                Listele();
            };
            this.Controls.Add(btnOkundu);

            Listele();
        }

        private void Listele()
        {
            _grid.DataSource = Veritabani.VeriGetir($@"
SELECT 
    Tarih AS Tarih,
    Mesaj AS Bildirim,
    CASE WHEN Okundu = 1 THEN 'Okundu' ELSE 'Yeni' END AS Durum
FROM Bildirimler
WHERE KullaniciAdi='{_kullaniciAdi}'
ORDER BY Tarih DESC
");
        }
    }
}
