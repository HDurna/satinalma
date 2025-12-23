using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using satinalma.Data;
using satinalma.Models.Entities;
using satinalma.Models.Enums;
using System.Collections.Generic;

namespace satinalma.Forms
{
    public class TalepEkleForm : Form
    {
        // --- Form Kontrolleri ---
        private ComboBox _cmbBirim;
        private TextBox _txtTalepNo;
        private ComboBox _cmbTalepEden;
        private DateTimePicker _dtpTarih;
        private DateTimePicker _dtpSevkTarihi;
        private ComboBox _cmbTip;
        private CheckBox _chkEkap;
        private TextBox _txtAciklama;

        // Ürün Ekleme Kısmı
        private TextBox _txtSecilenUrunAdi;
        private int _secilenUrunId = 0;
        private Button _btnBul;
        private NumericUpDown _numMiktar;
        private ComboBox _cmbOlcu;
        private Button _btnListeyeEkle;
        private Button _btnSatirSil;

        // Alt Kısım
        private DataGridView _gridSepet;
        private Button _btnKaydet;

        // Değişkenler
        private List<TalepDetay> _talepDetaylari = new List<TalepDetay>();
        private readonly Kullanici _aktifKullanici;

        public TalepEkleForm(Kullanici aktifKullanici)
        {
            _aktifKullanici = aktifKullanici;

            FormGorunum.Uygula(this);
            this.Text = "Yeni Talep Oluştur";
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            InitializeControls();
            VerileriYukle();
        }

        private void InitializeControls()
        {
            // 1. GRUP: GENEL BİLGİLER
            GroupBox grpGenel = new GroupBox { Text = "Talep Genel Bilgileri", Location = new Point(10, 10), Size = new Size(960, 180), Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            int y1 = 30;
            grpGenel.Controls.Add(new Label { Text = "Talep Birimi:", Location = new Point(20, y1), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });
            _cmbBirim = new ComboBox { Location = new Point(110, y1 - 3), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            _cmbBirim.SelectedIndexChanged += _cmbBirim_SelectedIndexChanged; // NUMARA ÜRETME OLAYI
            grpGenel.Controls.Add(_cmbBirim);

            grpGenel.Controls.Add(new Label { Text = "Talep No:", Location = new Point(350, y1), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });
            _txtTalepNo = new TextBox { Location = new Point(420, y1 - 3), Width = 150, ReadOnly = true, BackColor = Color.LightYellow, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpGenel.Controls.Add(_txtTalepNo);

            grpGenel.Controls.Add(new Label { Text = "Talep Eden:", Location = new Point(600, y1), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });
            _cmbTalepEden = new ComboBox { Location = new Point(690, y1 - 3), Width = 200, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpGenel.Controls.Add(_cmbTalepEden);

            int y2 = 70;
            grpGenel.Controls.Add(new Label { Text = "Talep Tarihi:", Location = new Point(20, y2), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });
            _dtpTarih = new DateTimePicker { Location = new Point(110, y2 - 3), Width = 200, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpGenel.Controls.Add(_dtpTarih);

            grpGenel.Controls.Add(new Label { Text = "Sevk Tarihi:", Location = new Point(350, y2), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });
            _dtpSevkTarihi = new DateTimePicker { Location = new Point(420, y2 - 3), Width = 150, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpGenel.Controls.Add(_dtpSevkTarihi);

            int y3 = 110;
            grpGenel.Controls.Add(new Label { Text = "Talep Tipi:", Location = new Point(20, y3), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });
            _cmbTip = new ComboBox { Location = new Point(110, y3 - 3), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            _cmbTip.DataSource = Enum.GetValues(typeof(TalepTipi));
            grpGenel.Controls.Add(_cmbTip);

            _chkEkap = new CheckBox { Text = "EKAP Yapıldı mı?", Location = new Point(250, y3), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpGenel.Controls.Add(_chkEkap);

            grpGenel.Controls.Add(new Label { Text = "Açıklama:", Location = new Point(420, y3), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });
            _txtAciklama = new TextBox { Location = new Point(480, y3 - 3), Width = 410, Height = 40, Multiline = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpGenel.Controls.Add(_txtAciklama);

            this.Controls.Add(grpGenel);

            // 2. GRUP: MALZEME LİSTESİ OLUŞTUR
            GroupBox grpDetay = new GroupBox { Text = "Malzeme Listesi Oluştur", Location = new Point(10, 200), Size = new Size(960, 80), Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            grpDetay.Controls.Add(new Label { Text = "Stok Kartı:", Location = new Point(20, 35), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });

            _txtSecilenUrunAdi = new TextBox { Location = new Point(90, 32), Width = 300, ReadOnly = true, BackColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpDetay.Controls.Add(_txtSecilenUrunAdi);

            _btnBul = new Button { Text = "BUL", Location = new Point(400, 30), Width = 50, Height = 26, BackColor = Color.SteelBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 8, FontStyle.Bold) };
            _btnBul.Click += BtnBul_Click;
            grpDetay.Controls.Add(_btnBul);

            grpDetay.Controls.Add(new Label { Text = "Miktar:", Location = new Point(470, 35), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) });
            _numMiktar = new NumericUpDown { Location = new Point(520, 32), Width = 80, Maximum = 10000, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpDetay.Controls.Add(_numMiktar);

            _cmbOlcu = new ComboBox { Location = new Point(610, 32), Width = 80, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            grpDetay.Controls.Add(_cmbOlcu);

            _btnListeyeEkle = new Button { Text = "EKLE", Location = new Point(710, 30), Width = 80, BackColor = Color.Orange, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _btnListeyeEkle.Click += BtnListeyeEkle_Click;
            grpDetay.Controls.Add(_btnListeyeEkle);

            _btnSatirSil = new Button { Text = "SATIR SİL", Location = new Point(800, 30), Width = 90, BackColor = Color.IndianRed, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _btnSatirSil.Click += BtnSatirSil_Click;
            grpDetay.Controls.Add(_btnSatirSil);

            this.Controls.Add(grpDetay);

            // 3. GRİD
            _gridSepet = new DataGridView
            {
                Location = new Point(10, 290),
                Size = new Size(960, 350),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };
            _gridSepet.Columns.Add("UrunAdi", "Ürün Adı");
            _gridSepet.Columns.Add("Miktar", "Miktar");
            _gridSepet.Columns.Add("Birim", "Birim");

            this.Controls.Add(_gridSepet);

            // 4. KAYDET
            _btnKaydet = new Button
            {
                Text = "TALEBİ TAMAMLA VE KAYDET",
                Location = new Point(690, 650),
                Size = new Size(280, 50),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            _btnKaydet.Click += BtnKaydet_Click;
            this.Controls.Add(_btnKaydet);
        }

        private void VerileriYukle()
        {
            using (var db = new SatinAlmaDbContext())
            {
                // 1. SORUN DÜZELTİLDİ: Sadece senin tanımladığın birimleri getiriyoruz.
                // Otomatik ekleme kodu kaldırıldı.
                var birimler = db.Birimler.ToList();

                _cmbBirim.DataSource = new List<Birim>(birimler);
                _cmbBirim.DisplayMember = "BirimAdi";
                _cmbBirim.ValueMember = "Id";
                _cmbBirim.SelectedIndex = -1;

                // Ölçü Birimlerini Yükle
                var olcuBirimleri = db.OlcuBirimleri.ToList();
                _cmbOlcu.DataSource = olcuBirimleri;
                _cmbOlcu.DisplayMember = "BirimAdi";
                _cmbOlcu.ValueMember = "BirimAdi";

                // Talep Edenleri Yükle
                var kullanicilar = db.Kullanicilar.Select(k => k.AdSoyad).ToList();
                _cmbTalepEden.Items.Clear();
                foreach (var k in kullanicilar) _cmbTalepEden.Items.Add(k);

                // Varsayılan
                if (_aktifKullanici != null)
                    _cmbTalepEden.SelectedItem = _aktifKullanici.AdSoyad;
            }
        }

        // 2. SORUN DÜZELTİLDİ: SIRALI NUMARA ÜRETME (Örn: INS-20251223-1)
        private void _cmbBirim_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cmbBirim.SelectedItem == null) return;

            var secilenBirim = (Birim)_cmbBirim.SelectedItem;
            string birimAdi = secilenBirim.BirimAdi.ToUpper();

            // Kısaltma (İlk 3 harf)
            string onEk = "GEN";
            if (birimAdi.Length >= 3) onEk = birimAdi.Substring(0, 3);

            // Türkçe karakter temizliği
            onEk = onEk.Replace("İ", "I").Replace("Ş", "S").Replace("Ç", "C").Replace("Ğ", "G").Replace("Ü", "U").Replace("Ö", "O");

            // Format: ON_EK-Tarih (Örn: INS-20251223)
            string tarihKodu = DateTime.Now.ToString("yyyyMMdd");
            string prefix = $"{onEk}-{tarihKodu}-";

            // Veritabanından son numarayı bulalım
            using (var db = new SatinAlmaDbContext())
            {
                // Bu prefix ile başlayan en son talebi bul
                var sonTalep = db.TalepBasliklar
                                 .Where(t => t.TalepNo.StartsWith(prefix))
                                 .OrderByDescending(t => t.Id) // ID'si en büyük olan (en son eklenen)
                                 .FirstOrDefault();

                int yeniSiraNo = 1;

                if (sonTalep != null)
                {
                    // Mevcut numaranın sonundaki sayıyı al (Örn: INS-20251223-5 -> 5'i al)
                    string[] parcalar = sonTalep.TalepNo.Split('-');
                    if (parcalar.Length > 0)
                    {
                        if (int.TryParse(parcalar[parcalar.Length - 1], out int sonNo))
                        {
                            yeniSiraNo = sonNo + 1;
                        }
                    }
                }

                // Yeni numara (Örn: INS-20251223-1)
                _txtTalepNo.Text = $"{prefix}{yeniSiraNo}";
            }
        }

        private void BtnBul_Click(object sender, EventArgs e)
        {
            using (var db = new SatinAlmaDbContext())
            {
                // 3. SORUN DÜZELTİLDİ: Aktif filtresini kaldırdım, hepsi gelsin.
                var urunler = db.Urunler
                                .Select(u => new { u.Id, u.UrunAdi })
                                .OrderBy(u => u.UrunAdi)
                                .ToList();

                // Seçim Formu
                Form secimForm = new Form { Text = "Ürün Seç", Size = new Size(400, 150), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedToolWindow };
                ComboBox cmbUrunler = new ComboBox { Location = new Point(10, 20), Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbUrunler.DataSource = urunler;
                cmbUrunler.DisplayMember = "UrunAdi";
                cmbUrunler.ValueMember = "Id";

                Button btnSec = new Button { Text = "SEÇ", Location = new Point(150, 60), DialogResult = DialogResult.OK, BackColor = Color.SteelBlue, ForeColor = Color.White };
                secimForm.Controls.Add(cmbUrunler);
                secimForm.Controls.Add(btnSec);

                if (secimForm.ShowDialog() == DialogResult.OK)
                {
                    if (cmbUrunler.SelectedItem != null)
                    {
                        var item = cmbUrunler.SelectedItem;
                        var idProp = item.GetType().GetProperty("Id");
                        var adProp = item.GetType().GetProperty("UrunAdi");

                        _secilenUrunId = (int)idProp.GetValue(item, null);
                        _txtSecilenUrunAdi.Text = adProp.GetValue(item, null).ToString();
                        _numMiktar.Focus();
                    }
                }
            }
        }

        private void BtnListeyeEkle_Click(object sender, EventArgs e)
        {
            if (_secilenUrunId == 0)
            {
                MessageBox.Show("Lütfen 'BUL' butonu ile bir ürün seçiniz.");
                return;
            }

            if (_numMiktar.Value <= 0)
            {
                MessageBox.Show("Miktar giriniz.");
                return;
            }

            _talepDetaylari.Add(new TalepDetay
            {
                UrunId = _secilenUrunId,
                UrunAdi = _txtSecilenUrunAdi.Text,
                Miktar = (double)_numMiktar.Value,
                Birim = _cmbOlcu.Text,
                SiparisMiktari = 0,
                KalanMiktar = (double)_numMiktar.Value
            });

            _gridSepet.Rows.Add(_txtSecilenUrunAdi.Text, _numMiktar.Value, _cmbOlcu.Text);

            _secilenUrunId = 0;
            _txtSecilenUrunAdi.Clear();
            _numMiktar.Value = 1;
        }

        private void BtnSatirSil_Click(object sender, EventArgs e)
        {
            if (_gridSepet.SelectedRows.Count > 0)
            {
                int index = _gridSepet.SelectedRows[0].Index;
                _gridSepet.Rows.RemoveAt(index);
                _talepDetaylari.RemoveAt(index);
            }
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (_talepDetaylari.Count == 0)
            {
                MessageBox.Show("Listeye hiç ürün eklemediniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_cmbBirim.Text))
            {
                MessageBox.Show("Lütfen 'Talep Birimi' seçiniz.");
                return;
            }

            try
            {
                using (var db = new SatinAlmaDbContext())
                {
                    // Kaydetmeden önce numarayı son bir kez daha kontrol et (Çakışma olmasın)
                    // (Aynı anda iki kişi açarsa diye güvenlik önlemi)
                    _cmbBirim_SelectedIndexChanged(null, null);

                    var talep = new TalepBaslik
                    {
                        TalepNo = _txtTalepNo.Text,
                        TalepTarihi = _dtpTarih.Value,
                        SevkTarihi = _dtpSevkTarihi.Value,
                        TalepBirimi = _cmbBirim.Text,
                        TalepEden = _cmbTalepEden.Text,
                        EkapYapildi = _chkEkap.Checked,
                        Aciklama = _txtAciklama.Text ?? "",
                        Tip = (TalepTipi)_cmbTip.SelectedItem,
                        Durum = TalepDurumu.Acik,
                        KullaniciId = _aktifKullanici.Id > 0 ? _aktifKullanici.Id : 1
                    };

                    db.TalepBasliklar.Add(talep);
                    db.SaveChanges();

                    foreach (var detay in _talepDetaylari)
                    {
                        detay.TalepBaslikId = talep.Id;
                        db.TalepDetaylar.Add(detay);
                    }
                    db.SaveChanges();

                    MessageBox.Show($"Talep {_txtTalepNo.Text} numarasıyla başarıyla oluşturuldu.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
    }
}