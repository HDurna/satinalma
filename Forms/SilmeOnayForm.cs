using System;
using System.Drawing;
using System.Windows.Forms;
using satinalma.Data;

namespace satinalma.Forms
{
    public class SilmeOnayForm : Form
    {
        private DataGridView _grid;

        public SilmeOnayForm()
        {
            this.Text = "Silme Onayı Bekleyen Talepler";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lbl = new Label
            {
                Text = "Aşağıdaki talepler personel tarafından silinmek isteniyor.",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lbl);

            // Onay Butonu
            Button btnOnayla = new Button
            {
                Text = "SİLMEYİ ONAYLA (KALICI SİL)",
                Location = new Point(10, 400),
                Width = 220,
                Height = 40,
                BackColor = Color.IndianRed,
                ForeColor = Color.White
            };
            btnOnayla.Click += (s, e) => IslemYap(true);
            this.Controls.Add(btnOnayla);

            // Red Butonu
            Button btnReddet = new Button
            {
                Text = "REDDET (SİLME)",
                Location = new Point(240, 400),
                Width = 150,
                Height = 40,
                BackColor = Color.Gray,
                ForeColor = Color.White
            };
            btnReddet.Click += (s, e) => IslemYap(false);
            this.Controls.Add(btnReddet);

            _grid = new DataGridView
            {
                Location = new Point(10, 40),
                Size = new Size(860, 350),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(_grid);

            Listele();
        }

        private void Listele()
        {
            _grid.DataSource = Veritabani.VeriGetir("SELECT Id, TalepNo, TalepBirimi, SilmeAciklamasi FROM TalepBaslik WHERE SilmeDurumu='TalepEdildi'");
        }

        private void IslemYap(bool onaylandi)
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir satır seçin.");
                return;
            }

            int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
            string no = _grid.SelectedRows[0].Cells["TalepNo"].Value.ToString();

            string adminKullanici = Program.AktifKullanici != null
                ? Program.AktifKullanici.KullaniciAdi
                : "Admin";

            if (onaylandi)
            {
                if (MessageBox.Show($"{no} nolu talep TAMAMEN silinecek. Onaylıyor musunuz?",
                                     "Son Karar",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // Log: Onay
                    Veritabani.SorguCalistir($@"
INSERT INTO SilmeLog (TalepBaslikId, TalepNo, IslemTarihi, IslemTipi, KullaniciAdi, Aciklama)
VALUES ({id}, '{no}', '{DateTime.Now:yyyy-MM-dd HH:mm}', 'Onay', '{adminKullanici}', 'Silme onaylandı ve kayıt kalıcı olarak silindi')
");

                    // Kayıtları sil
                    Veritabani.SorguCalistir($"DELETE FROM TalepDetay WHERE TalepBaslikId={id}");
                    Veritabani.SorguCalistir($"DELETE FROM TalepBaslik WHERE Id={id}");
                    MessageBox.Show("Silme işlemi onaylandı ve kayıt silindi.");
                }
            }
            else
            {
                // REDDEDİLDİ: Durumu temizle, kayıt yaşamaya devam ediyor
                Veritabani.SorguCalistir($"UPDATE TalepBaslik SET SilmeDurumu='Yok', SilmeAciklamasi='' WHERE Id={id}");

                // Log: Red
                Veritabani.SorguCalistir($@"
INSERT INTO SilmeLog (TalepBaslikId, TalepNo, IslemTarihi, IslemTipi, KullaniciAdi, Aciklama)
VALUES ({id}, '{no}', '{DateTime.Now:yyyy-MM-dd HH:mm}', 'Red', '{adminKullanici}', 'Silme talebi reddedildi')
");

                // Silme talebini oluşturan kişiyi bul (en son 'Talep' kaydı)
                object talepEdenObj = Veritabani.TekDegerGetir($@"
SELECT KullaniciAdi FROM SilmeLog
WHERE TalepBaslikId={id} AND IslemTipi='Talep'
ORDER BY Id DESC LIMIT 1
");

                if (talepEdenObj != null && talepEdenObj != DBNull.Value)
                {
                    string talepEden = talepEdenObj.ToString();
                    string mesaj = $"{no} nolu talebin için yaptığın silme talebi, yönetici tarafından REDDEDİLDİ.";

                    Veritabani.SorguCalistir($@"
INSERT INTO Bildirimler (KullaniciAdi, Mesaj, Tarih, Okundu)
VALUES ('{talepEden}', '{mesaj}', '{DateTime.Now:yyyy-MM-dd HH:mm}', 0)
");
                }

                MessageBox.Show("Silme talebi reddedildi, kayıt sistemde kaldı.");
            }

            Listele();
        }
    }
}
