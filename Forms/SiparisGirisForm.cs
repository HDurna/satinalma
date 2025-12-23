using satinalma.Data;
using satinalma.Models.Entities;
using satinalma.Models.Enums;
using System;
using System.Data;
using System.Linq;

namespace satinalma.Forms
{
    public class SiparisGirisForm : Form
    {
        private ComboBox _cmbTedarikci;
        private ComboBox _cmbTalep;
        private TextBox _txtSiparisNo;
        private DateTimePicker _dtpTarih;
        private DataGridView _gridUrunler;
        private Button _btnKaydet;

        private int _seciliTalepId = 0;
        private readonly Kullanici _aktifKullanici;
        private readonly int _duzenlenecekSiparisId;

        // Constructor
        public SiparisGirisForm(Kullanici aktifKullanici, int siparisId = 0)
        {
            _aktifKullanici = aktifKullanici;
            _duzenlenecekSiparisId = siparisId;

            FormGorunum.Uygula(this);
            Text = siparisId > 0 ? "Sipariş Güncelle" : "Yeni Sipariş Oluştur";
            Size = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;

            InitializeControls();
            VerileriYukle();
        }

        private void InitializeControls()
        {
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(10)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Grid
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));  // Footer

            // --- Header Alanı ---
            var pnlHeader = new Panel { Dock = DockStyle.Fill };

            var lblTedarikci = new Label { Text = "Tedarikçi:", Location = new Point(10, 15), AutoSize = true };
            _cmbTedarikci = new ComboBox { Location = new Point(100, 12), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblTalep = new Label { Text = "Talep Seç:", Location = new Point(320, 15), AutoSize = true };
            _cmbTalep = new ComboBox { Location = new Point(400, 12), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbTalep.SelectedIndexChanged += _cmbTalep_SelectedIndexChanged;

            var lblSiparisNo = new Label { Text = "Sipariş No:", Location = new Point(10, 50), AutoSize = true };
            _txtSiparisNo = new TextBox { Location = new Point(100, 47), Width = 200, ReadOnly = true };

            var lblTarih = new Label { Text = "Sipariş Tarihi:", Location = new Point(320, 50), AutoSize = true };
            _dtpTarih = new DateTimePicker { Location = new Point(400, 47), Width = 200, Format = DateTimePickerFormat.Short };

            pnlHeader.Controls.AddRange(new Control[] { lblTedarikci, _cmbTedarikci, lblTalep, _cmbTalep, lblSiparisNo, _txtSiparisNo, lblTarih, _dtpTarih });

            // --- Grid Alanı ---
            _gridUrunler = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            // --- Footer Alanı ---
            var pnlFooter = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            _btnKaydet = new Button { Text = "Siparişi Oluştur", Width = 150, Height = 40, BackColor = Color.SteelBlue, ForeColor = Color.White };
            _btnKaydet.Click += _btnKaydet_Click;
            pnlFooter.Controls.Add(_btnKaydet);

            mainLayout.Controls.Add(pnlHeader, 0, 0);
            mainLayout.Controls.Add(_gridUrunler, 0, 1);
            mainLayout.Controls.Add(pnlFooter, 0, 2);

            Controls.Add(mainLayout);
        }

        private void VerileriYukle()
        {
            using (var db = new SatinAlmaDbContext())
            {
                // Tedarikçileri Yükle
                var tedarikciler = db.TedarikciFirmalar
                                     .Where(t => t.Aktif)
                                     .Select(t => new { t.Id, t.FirmaAdi })
                                     .ToList();
                _cmbTedarikci.DataSource = tedarikciler;
                _cmbTedarikci.DisplayMember = "FirmaAdi";
                _cmbTedarikci.ValueMember = "Id";

                // DUZELTME: Eski kodda 'TalepTarihi' vardi, artik 'Tarih' oldu.
                // DUZELTME: Enum karsilastirmasi duzeltildi.
                var talepler = db.TalepBasliklar
                                 .Where(t => t.Durum == TalepDurumu.Acik || t.Durum == TalepDurumu.Bekliyor)
                                 .Select(t => new { t.Id, t.TalepNo, t.Aciklama })
                                 .OrderByDescending(t => t.Id)
                                 .ToList();

                _cmbTalep.DataSource = talepler;
                _cmbTalep.DisplayMember = "TalepNo";
                _cmbTalep.ValueMember = "Id";
                _cmbTalep.SelectedIndex = -1;
            }
        }

        private void _cmbTalep_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cmbTalep.SelectedValue == null) return;

            if (int.TryParse(_cmbTalep.SelectedValue.ToString(), out int talepId))
            {
                _seciliTalepId = talepId;

                // Reflection ile TalepNo özelliğini al
                var secilenTalepNesnesi = _cmbTalep.SelectedItem;
                string talepNo = secilenTalepNesnesi.GetType().GetProperty("TalepNo").GetValue(secilenTalepNesnesi, null).ToString();

                _txtSiparisNo.Text = $"{talepNo}-{DateTime.Now.Month:00}";

                using (var db = new SatinAlmaDbContext())
                {
                    // DUZELTME: 'Urun' navigasyonu yerine 'UrunAdi' kullandik (TalepDetay tablosunda UrunAdi var)
                    var detaylar = db.TalepDetaylar
                                     .Where(d => d.TalepBaslikId == talepId)
                                     .Select(d => new
                                     {
                                         d.Id,
                                         Urun = d.UrunAdi,
                                         Miktar = d.Miktar,
                                         Birim = d.Birim
                                     })
                                     .ToList();

                    _gridUrunler.DataSource = detaylar;
                }
            }
        }

        private void _btnKaydet_Click(object? sender, EventArgs e)
        {
            if (_seciliTalepId == 0 || _cmbTedarikci.SelectedValue == null)
            {
                MessageBox.Show("Lütfen talep ve tedarikçi seçiniz.");
                return;
            }

            try
            {
                using (var db = new SatinAlmaDbContext())
                {
                    var siparis = new SiparisBaslik
                    {
                        SiparisNo = _txtSiparisNo.Text,
                        SiparisTarihi = _dtpTarih.Value,
                        TedarikciFirmaId = (int)_cmbTedarikci.SelectedValue,
                        Durum = SiparisDurumu.Hazirlaniyor,
                        // Kullanici nesnesi yerine string alan kullaniliyor (eski yapiya uyum icin)
                        OlusturanKullanici = _aktifKullanici.AdSoyad ?? "Sistem",
                        ToplamTutar = 0
                    };

                    db.SiparisBasliklar.Add(siparis);
                    db.SaveChanges();

                    // Grid'den SiparisDetay olustur
                    foreach (DataGridViewRow row in _gridUrunler.Rows)
                    {
                        var detay = new SiparisDetay
                        {
                            SiparisBaslikId = siparis.Id,
                            UrunAdi = row.Cells["Urun"].Value.ToString(),
                            Miktar = Convert.ToDouble(row.Cells["Miktar"].Value),
                            BirimFiyat = 0
                        };
                        db.SiparisDetaylar.Add(detay);
                    }

                    // Talebi Kapat
                    var talep = db.TalepBasliklar.Find(_seciliTalepId);
                    if (talep != null)
                    {
                        talep.Durum = TalepDurumu.Kapali;
                    }
                    db.SaveChanges();

                    MessageBox.Show("Sipariş başarıyla oluşturuldu.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}