using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using satinalma.Data;
using satinalma.Models.Entities;
using satinalma.Models.Enums;

namespace satinalma.Forms
{
    public class TalepListesiForm : Form
    {
        private readonly Kullanici _aktifKullanici;

        private DataGridView _grid = null!;
        private Button _btnYeni = null!;
        private Button _btnEkap = null!;
        private Button _btnDuzelt = null!;
        private Button _btnSil = null!;

        public TalepListesiForm(Kullanici aktifKullanici)
        {
            _aktifKullanici = aktifKullanici;

            // Ortak görünüm
            FormGorunum.Uygula(this);
            this.Text = "Talep Listesi Yönetimi";

            ArayuzOlustur();
            OlaylariBagla();
            Listele();
        }

        private void ArayuzOlustur()
        {
            // Ana layout: 2 satır (üst: toolbar, alt: grid)
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));            // toolbar
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));        // grid
            this.Controls.Add(mainLayout);

            // ---- ÜST BÖLÜM (toolbar + bilgi etiketi) ----
            var topLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10, 10, 10, 5)
            };
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // butonlar
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));   // bilgi etiketi
            mainLayout.Controls.Add(topLayout, 0, 0);

            // Butonlar paneli
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };

            _btnYeni = ButonOlustur("+ YENİ TALEP", Color.SeaGreen);
            _btnEkap = ButonOlustur("EKAP HAZIRLA", Color.DarkOrange);
            _btnDuzelt = ButonOlustur("DÜZELT", Color.DodgerBlue);
            _btnSil = ButonOlustur("SİL", Color.IndianRed);

            buttonsPanel.Controls.Add(_btnYeni);
            buttonsPanel.Controls.Add(_btnEkap);
            buttonsPanel.Controls.Add(_btnDuzelt);
            buttonsPanel.Controls.Add(_btnSil);

            topLayout.Controls.Add(buttonsPanel, 0, 0);

            // Bilgi etiketi
            var lblBilgi = new Label
            {
                Text = "* Detay için çift tıklayın.",
                AutoSize = true,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.DimGray
            };
            topLayout.Controls.Add(lblBilgi, 1, 0);

            // ---- ALT BÖLÜM (GRID) ----
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            mainLayout.Controls.Add(_grid, 0, 1);
        }

        private Button ButonOlustur(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Width = 120,
                Height = 35,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 8, 0)
            };
        }

        private void OlaylariBagla()
        {
            _btnYeni.Click += (s, e) => YeniTalep();
            _btnDuzelt.Click += (s, e) => Duzenle();
            _btnEkap.Click += (s, e) => EkapHazirla();
            _btnSil.Click += (s, e) => Sil();

            _grid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    DetayAc(e.RowIndex);
            };
        }

        private void Listele()
        {
            string sql = @"
        SELECT 
            Id,
            TalepNo,
            TalepTarihi,
            SevkTarihi,
            TalepBirimi,
            Tip,
            EkapYapildi,
            Durum,
            SilmeDurumu,
            SilmeAciklamasi
        FROM TalepBaslik
        ORDER BY Id DESC";

            DataTable dt = Veritabani.VeriGetir(sql);

            if (_grid == null)
                return;

            _grid.DataSource = dt;

            if (dt == null)
                return;

            // -----------------------------
            // Kolon ayarlarını güvenli yap
            // -----------------------------
            void KolonAyarla(string kolonAdi, string? header = null, int? displayIndex = null)
            {
                var columns = _grid.Columns;
                if (columns == null || !columns.Contains(kolonAdi))
                    return;

                var col = columns[kolonAdi];
                if (col == null)
                    return;

                if (displayIndex.HasValue)
                {
                    try
                    {
                        col.DisplayIndex = displayIndex.Value;
                    }
                    catch
                    {
                        // WinForms bazen burada da takılabiliyor, umursamıyoruz.
                    }
                }

                if (!string.IsNullOrWhiteSpace(header))
                    col.HeaderText = header!;
            }

            // ID kolonu – sadece başa al, genişlik elleme
            KolonAyarla("Id", "Id", displayIndex: 0);

            // Diğer kolonlar – başlık düzelt
            KolonAyarla("TalepNo", "Talep No");
            KolonAyarla("TalepTarihi", "Talep Tarihi");
            KolonAyarla("SevkTarihi", "Sevk Tarihi");
            KolonAyarla("TalepBirimi", "Talep Birimi");
            KolonAyarla("Tip", "Tip");
            KolonAyarla("EkapYapildi", "EKAP");
            KolonAyarla("Durum", "Durum");
            KolonAyarla("SilmeDurumu", "Silme Durumu");
            KolonAyarla("SilmeAciklamasi", "Silme Açıklaması");
        }

        private int? SeciliTalepIdAl()
        {
            if (_grid.CurrentRow == null) return null;
            object idDeger = _grid.CurrentRow.Cells["Id"].Value;
            if (idDeger == null || idDeger == DBNull.Value) return null;
            return Convert.ToInt32(idDeger);
        }

        private void YeniTalep()
        {
            using (var frm = new TalepEkleForm())
            {
                frm.ShowDialog(this);
            }
            Listele();
        }

        private void Duzenle()
        {
            int? id = SeciliTalepIdAl();
            if (id == null)
            {
                MessageBox.Show("Lütfen önce bir talep seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var frm = new TalepEkleForm(id.Value))
            {
                frm.ShowDialog(this);
            }
            Listele();
        }

        private void DetayAc(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _grid.Rows.Count) return;

            int id = Convert.ToInt32(_grid.Rows[rowIndex].Cells["Id"].Value);
            string talepNo = Convert.ToString(_grid.Rows[rowIndex].Cells["TalepNo"].Value) ?? "";

            using (var frm = new TalepDetayForm(id, talepNo))
            {
                frm.ShowDialog(this);
            }
        }

        private void EkapHazirla()
        {
            int? id = SeciliTalepIdAl();
            if (id == null)
            {
                MessageBox.Show("Lütfen önce bir talep seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show(
                "Bu talebi EKAP için işaretlemek istiyor musunuz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Veritabani.SorguCalistir($"UPDATE TalepBaslik SET EkapYapildi = 1 WHERE Id = {id.Value}");
                Listele();
            }
        }

        private void Sil()
        {
            int? id = SeciliTalepIdAl();
            if (id == null)
            {
                MessageBox.Show("Lütfen önce bir talep seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Admin ise direkt silme
            if (_aktifKullanici.Yetki == Rol.Admin)
            {
                var dr = MessageBox.Show(
                    "Bu talep ve tüm satırları kalıcı olarak silinecek. Emin misiniz?",
                    "Kalıcı Silme Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    // Önce detay satırlarını sil
                    Veritabani.SorguCalistir($"DELETE FROM TalepDetay WHERE TalepBaslikId = {id.Value}");
                    // Sonra başlığı sil
                    Veritabani.SorguCalistir($"DELETE FROM TalepBaslik WHERE Id = {id.Value}");
                    MessageBox.Show("Talep silindi.", "Bilgi");
                    Listele();
                }
            }
            else
            {
                // Satınalma veya birim personeli ise: silme talebi
                string aciklama = InputBoxGoster(
                    "Silme Gerekçesi",
                    "Bu talebi neden silmek istiyorsunuz?");

                if (!string.IsNullOrWhiteSpace(aciklama))
                {
                    aciklama = aciklama.Replace("'", "''"); // SQL injection’a karşı basit kaçış
                    Veritabani.SorguCalistir(
                        $"UPDATE TalepBaslik SET SilmeDurumu='TalepEdildi', SilmeAciklamasi='{aciklama}' WHERE Id={id.Value}");

                    MessageBox.Show("Silme talebiniz yönetici onayına gönderildi.",
                        "Talep Alındı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Listele();
                }
            }
        }

        // Basit InputBox helper’ı
        private static string InputBoxGoster(string baslik, string soru)
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 420;
                prompt.Height = 200;
                prompt.Text = baslik;
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                Label textLabel = new Label
                {
                    Left = 20,
                    Top = 20,
                    Text = soru,
                    AutoSize = true
                };
                TextBox textBox = new TextBox
                {
                    Left = 20,
                    Top = 50,
                    Width = 360
                };
                Button confirmation = new Button
                {
                    Text = "Gönder",
                    Left = 20,
                    Top = 90,
                    Width = 100,
                    DialogResult = DialogResult.OK,
                    BackColor = Color.SeaGreen,
                    ForeColor = Color.White
                };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
            }
        }
    }
}
