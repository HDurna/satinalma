using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using satinalma.Data;
using satinalma.Models.Entities;
using satinalma.Models.Enums;

namespace satinalma.Forms
{
    public class SiparisListesiForm : Form
    {
        private DataGridView _grid = null!;
        private Button _btnYeni, _btnDetay, _btnDuzelt, _btnSil = null!;
        private readonly Kullanici _aktifKullanici;

        public SiparisListesiForm(Kullanici aktifKullanici)
        {
            _aktifKullanici = aktifKullanici;
            FormGorunum.Uygula(this);
            this.Text = "Sipariş Listesi";
            BilesenleriOlustur();
            ListeyiYukle();
        }

        private void BilesenleriOlustur()
        {
            // Grid
            _grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(1000, 500),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _grid.DoubleClick += Grid_DoubleClick;

            // Buttons
            _btnYeni = YeniButon("Yeni Sipariş", Color.MediumSeaGreen);
            _btnYeni.Location = new Point(20, 540);
            _btnYeni.Click += BtnYeni_Click;

            _btnDetay = YeniButon("Detay Gör", Color.SteelBlue);
            _btnDetay.Location = new Point(160, 540);
            _btnDetay.Click += BtnDetay_Click;

            _btnDuzelt = YeniButon("Güncelle", Color.DarkOrange);
            _btnDuzelt.Location = new Point(300, 540);
            _btnDuzelt.Click += BtnDuzelt_Click;

            _btnSil = YeniButon("Sil", Color.Crimson);
            _btnSil.Location = new Point(440, 540);
            _btnSil.Click += BtnSil_Click;

            this.Controls.AddRange(new Control[] { _grid, _btnYeni, _btnDetay, _btnDuzelt, _btnSil });
        }

        private Button YeniButon(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = 130,
                Height = 35,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void ListeyiYukle()
        {
            using (var context = new SatinAlmaDbContext())
            {
                var list = context.SiparisBasliklar
                    .Select(s => new
                    {
                        s.Id,
                        s.SiparisNo,
                        s.SiparisTarihi,
                        TedarikçiFirma = s.TedarikçiFirma != null ? s.TedarikçiFirma.FirmaAdi : "",
                        s.ToplamTutar,
                        Durum = s.Durum.ToString(),
                        s.OlusturanKullanici
                    })
                    .OrderByDescending(s => s.SiparisTarihi)
                    .ToList();

                _grid.DataSource = list;
            }
        }

        private void BtnYeni_Click(object? sender, EventArgs e)
        {
            var form = new SiparisGirisForm(_aktifKullanici);
            form.ShowDialog();
            ListeyiYukle();
        }

        private void BtnDetay_Click(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
                var form = new SiparisDetayForm(id);
                form.ShowDialog();
            }
        }

        private void Grid_DoubleClick(object? sender, EventArgs e)
        {
            BtnDetay_Click(sender, e);
        }

        private void BtnDuzelt_Click(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz siparişi seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
            var form = new SiparisGirisForm(_aktifKullanici, id);
            form.ShowDialog();
            ListeyiYukle();
        }

        private void BtnSil_Click(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz siparişi seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
            string siparisNo = _grid.SelectedRows[0].Cells["SiparisNo"].Value?.ToString() ?? "";

            var result = MessageBox.Show(
                $"'{siparisNo}' numaralı siparişi silmek istediğinizden emin misiniz?\n\n" +
                "Bu işlem geri alınamaz ve sipariş detayları da silinecektir.",
                "Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var context = new SatinAlmaDbContext())
                    {
                        var siparis = context.SiparisBasliklar
                            .Where(s => s.Id == id)
                            .FirstOrDefault();

                        if (siparis != null)
                        {
                            // Detayları sil
                            var detaylar = context.SiparisDetaylar
                                .Where(d => d.SiparisBaslikId == id)
                                .ToList();

                            context.SiparisDetaylar.RemoveRange(detaylar);

                            // Başlığı sil
                            context.SiparisBasliklar.Remove(siparis);

                            context.SaveChanges();

                            MessageBox.Show("Sipariş başarıyla silindi.", "Başarılı",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            ListeyiYukle();
                        }
                        else
                        {
                            MessageBox.Show("Sipariş bulunamadı.", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Sipariş silinirken hata oluştu:\n{ex.Message}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
