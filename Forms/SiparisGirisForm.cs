using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using satinalma.Data;
using satinalma.Models.Entities;
using satinalma.Models.Enums;

namespace satinalma.Forms
{
    public class SiparisGirisForm : Form
    {
        private readonly Kullanici _aktifKullanici;
        private int _siparisId = 0; // 0 = yeni, >0 = düzenleme
        private TextBox _txtSiparisNo, _txtAciklama, _txtEkNo = null!;
        private DateTimePicker _dtpTarih = null!;
        private ComboBox _cmbTedarikci, _cmbTalep = null!;
        private DataGridView _gridKalemler = null!;
        private Label _lblToplamTutar, _lblTalepBilgi = null!;
        private Button _btnKaydet, _btnIptal, _btnTalepGetir = null!;

        public SiparisGirisForm(Kullanici aktifKullanici, int siparisId = 0)
        {
            _aktifKullanici = aktifKullanici;
            _siparisId = siparisId;
            FormGorunum.Uygula(this);
            this.Text = siparisId == 0 ? "Yeni Sipariş Girişi" : "Sipariş Düzenleme";
            BilesenleriOlustur();

            if (siparisId == 0)
                YeniSiparisNoUret();
            else
                SiparisiYukle();
        }

        private void BilesenleriOlustur()
        {
            // Sipariş No
            Label lblNo = new Label { Text = "Sipariş No:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            _txtSiparisNo = new TextBox { Location = new Point(150, 17), Width = 150, ReadOnly = true, BackColor = Color.LightGray };

            // Tarih
            Label lblTarih = new Label { Text = "Tarih:", Location = new Point(320, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            _dtpTarih = new DateTimePicker { Location = new Point(400, 17), Width = 150, Value = DateTime.Now };

            // Talep Seçimi (YENİ!)
            Label lblTalep = new Label { Text = "Talep Seç:", Location = new Point(20, 60), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            _cmbTalep = new ComboBox { Location = new Point(150, 57), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            _btnTalepGetir = YeniButon("Talep Ürünlerini Getir", Color.DodgerBlue);
            _btnTalepGetir.Location = new Point(460, 54);
            _btnTalepGetir.Width = 180;
            _btnTalepGetir.Click += BtnTalepGetir_Click;

            _lblTalepBilgi = new Label { Text = "", Location = new Point(650, 60), AutoSize = true, ForeColor = Color.Green, Font = new Font("Segoe UI", 9, FontStyle.Italic) };

            TalepleriYukle();

            // Tedarikçi
            Label lblTedarikci = new Label { Text = "Tedarikçi:", Location = new Point(20, 100), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            _cmbTedarikci = new ComboBox { Location = new Point(150, 97), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            TedarikcileriYukle();

            // EK No
            Label lblEk = new Label { Text = "EK/Sözleşme No:", Location = new Point(20, 140), AutoSize = true };
            _txtEkNo = new TextBox { Location = new Point(150, 137), Width = 150 };

            // Açıklama
            Label lblAciklama = new Label { Text = "Açıklama:", Location = new Point(320, 140), AutoSize = true };
            _txtAciklama = new TextBox { Location = new Point(400, 137), Width = 350, Height = 60, Multiline = true };

            // Kalemler Grid
            Label lblKalemler = new Label {
                Text = "Sipariş Kalemleri: (Talep miktarından fazla/eksik = renkli)",
                Location = new Point(20, 210),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            _gridKalemler = new DataGridView
            {
                Location = new Point(20, 240),
                Size = new Size(900, 250),
                AllowUserToAddRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            _gridKalemler.Columns.Add("TalepDetayId", "TalepDetayId");
            _gridKalemler.Columns["TalepDetayId"].Visible = false;
            _gridKalemler.Columns.Add("UrunAdi", "Ürün Adı");
            _gridKalemler.Columns.Add(new DataGridViewTextBoxColumn { Name = "TalepMiktar", HeaderText = "Talep Miktar", ValueType = typeof(double), ReadOnly = true });
            _gridKalemler.Columns.Add(new DataGridViewTextBoxColumn { Name = "Miktar", HeaderText = "Sipariş Miktar", ValueType = typeof(double) });
            _gridKalemler.Columns.Add("Birim", "Birim");
            _gridKalemler.Columns.Add(new DataGridViewTextBoxColumn { Name = "BirimFiyat", HeaderText = "Birim Fiyat", ValueType = typeof(decimal) });
            _gridKalemler.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tutar", HeaderText = "Tutar", ValueType = typeof(decimal), ReadOnly = true });

            _gridKalemler.CellValueChanged += GridKalemler_CellValueChanged;
            _gridKalemler.CellFormatting += GridKalemler_CellFormatting;

            // Toplam Tutar
            _lblToplamTutar = new Label {
                Text = "Toplam Tutar: 0.00 TL",
                Location = new Point(700, 500),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            // Buttons
            _btnKaydet = YeniButon("Kaydet", Color.Green);
            _btnKaydet.Location = new Point(620, 540);
            _btnKaydet.Width = 140;
            _btnKaydet.Height = 40;
            _btnKaydet.Click += BtnKaydet_Click;

            _btnIptal = YeniButon("İptal", Color.Gray);
            _btnIptal.Location = new Point(770, 540);
            _btnIptal.Width = 140;
            _btnIptal.Height = 40;
            _btnIptal.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblNo, _txtSiparisNo, lblTarih, _dtpTarih,
                lblTalep, _cmbTalep, _btnTalepGetir, _lblTalepBilgi,
                lblTedarikci, _cmbTedarikci, lblEk, _txtEkNo,
                lblAciklama, _txtAciklama, lblKalemler, _gridKalemler,
                _lblToplamTutar, _btnKaydet, _btnIptal
            });
        }

        private Button YeniButon(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private void YeniSiparisNoUret()
        {
            using (var context = new SatinAlmaDbContext())
            {
                var sonSiparis = context.SiparisBasliklar.OrderByDescending(s => s.Id).FirstOrDefault();
                int yeniId = (sonSiparis?.Id ?? 0) + 1;
                _txtSiparisNo.Text = $"SIP-{DateTime.Now:yyyyMMdd}-{yeniId:D4}";
            }
        }

        private void TalepleriYukle()
        {
            using (var context = new SatinAlmaDbContext())
            {
                // Sadece Açık veya SiparisVerildi durumundaki talepleri getir
                var talepler = context.TalepBasliklar
                    .Where(t => t.Durum == TalepDurumu.Acik || t.Durum == TalepDurumu.SiparisVerildi)
                    .Select(t => new
                    {
                        t.Id,
                        Display = t.TalepNo + " - " + t.TalepBirimi + " (" + t.TalepTarihi.ToShortDateString() + ")"
                    })
                    .ToList();

                _cmbTalep.DataSource = talepler;
                _cmbTalep.DisplayMember = "Display";
                _cmbTalep.ValueMember = "Id";

                if (talepler.Count == 0)
                {
                    _cmbTalep.Enabled = false;
                    _btnTalepGetir.Enabled = false;
                    _lblTalepBilgi.Text = "Açık talep yok!";
                    _lblTalepBilgi.ForeColor = Color.Red;
                }
            }
        }

        private void TedarikcileriYukle()
        {
            using (var context = new SatinAlmaDbContext())
            {
                var tedarikciler = context.TedarikciFirmalar.Where(t => t.Aktif).ToList();
                _cmbTedarikci.DataSource = tedarikciler;
                _cmbTedarikci.DisplayMember = "FirmaAdi";
                _cmbTedarikci.ValueMember = "Id";
            }
        }

        private void BtnTalepGetir_Click(object? sender, EventArgs e)
        {
            if (_cmbTalep.SelectedValue == null) return;

            int talepId = Convert.ToInt32(_cmbTalep.SelectedValue);

            using (var context = new SatinAlmaDbContext())
            {
                var detaylar = context.TalepDetaylar
                    .Where(d => d.TalepBaslikId == talepId)
                    .ToList();

                _gridKalemler.Rows.Clear();

                foreach (var detay in detaylar)
                {
                    int rowIndex = _gridKalemler.Rows.Add();
                    var row = _gridKalemler.Rows[rowIndex];

                    row.Cells["TalepDetayId"].Value = detay.Id;
                    row.Cells["UrunAdi"].Value = detay.UrunAdi;
                    row.Cells["TalepMiktar"].Value = detay.Miktar;
                    row.Cells["Miktar"].Value = detay.Miktar; // Başlangıçta aynı
                    row.Cells["Birim"].Value = detay.Birim;
                    row.Cells["BirimFiyat"].Value = 0m;
                    row.Cells["Tutar"].Value = 0m;
                }

                _lblTalepBilgi.Text = $"{detaylar.Count} kalem getirildi";
                _lblTalepBilgi.ForeColor = Color.Green;
            }
        }

        private void GridKalemler_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = _gridKalemler.Rows[e.RowIndex];

            // Miktar veya BirimFiyat değiştiğinde Tutar'ı hesapla
            if (e.ColumnIndex == _gridKalemler.Columns["Miktar"].Index ||
                e.ColumnIndex == _gridKalemler.Columns["BirimFiyat"].Index)
            {
                if (row.Cells["Miktar"].Value != null && row.Cells["BirimFiyat"].Value != null)
                {
                    try
                    {
                        double miktar = Convert.ToDouble(row.Cells["Miktar"].Value);
                        decimal birimFiyat = Convert.ToDecimal(row.Cells["BirimFiyat"].Value);
                        row.Cells["Tutar"].Value = (decimal)miktar * birimFiyat;
                        ToplamTutarHesapla();
                    }
                    catch { }
                }
            }
        }

        private void GridKalemler_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = _gridKalemler.Rows[e.RowIndex];

            // Renklendirme: Talep miktarı ile sipariş miktarını karşılaştır
            if (row.Cells["TalepMiktar"].Value != null && row.Cells["Miktar"].Value != null)
            {
                try
                {
                    double talepMiktar = Convert.ToDouble(row.Cells["TalepMiktar"].Value);
                    double siparisMiktar = Convert.ToDouble(row.Cells["Miktar"].Value);

                    if (siparisMiktar > talepMiktar)
                    {
                        // FAZLA SİPARİŞ - SARI
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                    }
                    else if (siparisMiktar < talepMiktar)
                    {
                        // EKSIK SİPARİŞ - AÇIK KIRMIZI
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else
                    {
                        // TAM UYUŞUYOR - BEYAZ
                        row.DefaultCellStyle.BackColor = Color.White;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
                catch { }
            }
        }

        private void ToplamTutarHesapla()
        {
            decimal toplam = 0;
            foreach (DataGridViewRow row in _gridKalemler.Rows)
            {
                if (row.Cells["Tutar"].Value != null)
                {
                    toplam += Convert.ToDecimal(row.Cells["Tutar"].Value);
                }
            }
            _lblToplamTutar.Text = $"Toplam Tutar: {toplam:N2} TL";
        }

        private void BtnKaydet_Click(object? sender, EventArgs e)
        {
            if (_cmbTedarikci.SelectedValue == null)
            {
                MessageBox.Show("Lütfen tedarikçi seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new SatinAlmaDbContext())
            {
                SiparisBaslik siparis;

                if (_siparisId == 0)
                {
                    // Yeni sipariş
                    siparis = new SiparisBaslik
                    {
                        SiparisNo = _txtSiparisNo.Text,
                        SiparisTarihi = _dtpTarih.Value,
                        TedarikciFirmaId = Convert.ToInt32(_cmbTedarikci.SelectedValue),
                        ToplamTutar = 0,
                        Durum = SiparisDurumu.Beklemede,
                        OlusturanKullanici = _aktifKullanici.KullaniciAdi,
                        Aciklama = _txtAciklama.Text,
                        EkNo = _txtEkNo.Text
                    };
                }
                else
                {
                    // Düzenleme
                    siparis = context.SiparisBasliklar.Find(_siparisId);
                    if (siparis == null)
                    {
                        MessageBox.Show("Sipariş bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    siparis.SiparisTarihi = _dtpTarih.Value;
                    siparis.TedarikciFirmaId = Convert.ToInt32(_cmbTedarikci.SelectedValue);
                    siparis.Aciklama = _txtAciklama.Text;
                    siparis.EkNo = _txtEkNo.Text;

                    // Eski kalemleri sil
                    var eskiDetaylar = context.SiparisDetaylar.Where(d => d.SiparisBaslikId == _siparisId).ToList();
                    context.SiparisDetaylar.RemoveRange(eskiDetaylar);
                }

                // Kalemleri ekle
                decimal toplamTutar = 0;
                foreach (DataGridViewRow row in _gridKalemler.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells["UrunAdi"].Value == null) continue;

                    var detay = new SiparisDetay
                    {
                        TalepDetayId = row.Cells["TalepDetayId"].Value != null ? Convert.ToInt32(row.Cells["TalepDetayId"].Value) : (int?)null,
                        UrunAdi = row.Cells["UrunAdi"].Value?.ToString() ?? "",
                        Miktar = Convert.ToDouble(row.Cells["Miktar"].Value ?? 0),
                        Birim = row.Cells["Birim"].Value?.ToString() ?? "",
                        BirimFiyat = Convert.ToDecimal(row.Cells["BirimFiyat"].Value ?? 0),
                        Tutar = Convert.ToDecimal(row.Cells["Tutar"].Value ?? 0)
                    };

                    toplamTutar += detay.Tutar;
                    siparis.SiparisDetaylari.Add(detay);
                }

                siparis.ToplamTutar = toplamTutar;

                if (_siparisId == 0)
                    context.SiparisBasliklar.Add(siparis);

                context.SaveChanges();

                MessageBox.Show("Sipariş kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void SiparisiYukle()
        {
            using (var context = new SatinAlmaDbContext())
            {
                var siparis = context.SiparisBasliklar.Find(_siparisId);
                if (siparis == null) return;

                _txtSiparisNo.Text = siparis.SiparisNo;
                _dtpTarih.Value = siparis.SiparisTarihi;
                _cmbTedarikci.SelectedValue = siparis.TedarikciFirmaId;
                _txtAciklama.Text = siparis.Aciklama;
                _txtEkNo.Text = siparis.EkNo;

                var detaylar = context.SiparisDetaylar.Where(d => d.SiparisBaslikId == _siparisId).ToList();
                _gridKalemler.Rows.Clear();

                foreach (var detay in detaylar)
                {
                    int rowIndex = _gridKalemler.Rows.Add();
                    var row = _gridKalemler.Rows[rowIndex];

                    row.Cells["TalepDetayId"].Value = detay.TalepDetayId;
                    row.Cells["UrunAdi"].Value = detay.UrunAdi;
                    row.Cells["TalepMiktar"].Value = 0; // Eski siparişlerde talep miktarı bilgisi yok
                    row.Cells["Miktar"].Value = detay.Miktar;
                    row.Cells["Birim"].Value = detay.Birim;
                    row.Cells["BirimFiyat"].Value = detay.BirimFiyat;
                    row.Cells["Tutar"].Value = detay.Tutar;
                }

                ToplamTutarHesapla();
            }
        }
    }
}
