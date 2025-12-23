using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using satinalma.Data;
using satinalma.Models.Entities;
using satinalma.Models.Enums;

namespace satinalma.Forms
{
    public class TalepEkleForm : Form
    {
        private int _duzenlenenId = 0;

        // Talep eden – aktif kullanıcıdan gelir, combobox'ta kullanılır
        private string _talepEden = "";

        private ComboBox _cmbBirim;
        private TextBox _txtTalepNo;
        private DateTimePicker _dtpTarih;
        private DateTimePicker _dtpSevkTarihi;
        private ComboBox _cmbTip;
        private CheckBox _chkEkap;

        // Yeni combo
        private ComboBox _cmbTalepEden;

        private TextBox _txtSecilenUrunAdi;
        private int _secilenUrunId = 0;

        private NumericUpDown _numMiktar;
        private ComboBox _cmbOlcu;
        private Button _btnListeyeEkle;
        private Button _btnSatirSil;
        private DataGridView _gridSepet;
        private Button _btnKaydet;

        private DataTable _dtSepet;

        public TalepEkleForm(int talepId = 0)
        {
            _duzenlenenId = talepId;

            // Aktif kullanıcı bilgisi
            _talepEden = Program.AktifKullanici?.AdSoyad ?? "";

            FormGorunum.Uygula(this);

            this.Text = _duzenlenenId > 0 ? "Talebi Düzenle" : "Yeni Talep Oluştur";
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterParent;

            SepetTablosunuHazirla();
            BilesenleriKur();
            VerileriYukle();

            if (_duzenlenenId > 0)
            {
                EskiVerileriGetir();
                _btnKaydet.Text = "GÜNCELLE VE KAYDET";
                _btnKaydet.BackColor = Color.DodgerBlue;
            }
        }

        private void SepetTablosunuHazirla()
        {
            _dtSepet = new DataTable();
            _dtSepet.Columns.Add("UrunId", typeof(int));
            _dtSepet.Columns.Add("UrunAdi", typeof(string));
            _dtSepet.Columns.Add("Miktar", typeof(double));
            _dtSepet.Columns.Add("Birim", typeof(string));
        }

        private void BilesenleriKur()
        {
            GroupBox grpBaslik = new GroupBox { Text = "Talep Genel Bilgileri", Location = new Point(10, 10), Size = new Size(960, 160) };
            int y = 30;

            Label lblBirim = new Label { Text = "Talep Birimi:", Location = new Point(20, y), AutoSize = true };
            _cmbBirim = new ComboBox { Location = new Point(110, y - 3), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbBirim.SelectedIndexChanged += (s, e) =>
            {
                if (_cmbBirim.SelectedItem != null && _duzenlenenId == 0)
                    _txtTalepNo.Text = Veritabani.YeniTalepNoUret(_cmbBirim.SelectedItem.ToString());
            };

            Label lblNo = new Label { Text = "Talep No:", Location = new Point(350, y), AutoSize = true };
            _txtTalepNo = new TextBox { Location = new Point(420, y - 3), Width = 150, ReadOnly = true, BackColor = Color.LightYellow };

            // -----------------------------------
            //         YENİ: TALEP EDEN
            // -----------------------------------
            Label lblTalepEden = new Label { Text = "Talep Eden:", Location = new Point(600, y), AutoSize = true };

            _cmbTalepEden = new ComboBox
            {
                Location = new Point(690, y - 3),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Personelleri doldur
            DataTable dtPersonel = Veritabani.VeriGetir("SELECT AdSoyad FROM Kullanicilar ORDER BY AdSoyad");
            foreach (DataRow row in dtPersonel.Rows)
                _cmbTalepEden.Items.Add(row["AdSoyad"].ToString());

            // Varsayılan olarak aktif kullanıcı
            if (!_cmbTalepEden.Items.Contains(_talepEden))
                _cmbTalepEden.Items.Add(_talepEden);

            _cmbTalepEden.SelectedItem = _talepEden;

            y += 40;

            Label lblTarih = new Label { Text = "Talep Tarihi:", Location = new Point(20, y), AutoSize = true };
            _dtpTarih = new DateTimePicker { Location = new Point(110, y - 3), Format = DateTimePickerFormat.Short, Width = 120 };

            Label lblSevk = new Label { Text = "Satınalma Sevk Tar.:", Location = new Point(280, y), AutoSize = true };
            _dtpSevkTarihi = new DateTimePicker { Location = new Point(420, y - 3), Format = DateTimePickerFormat.Short, Width = 120 };

            y += 40;
            Label lblTip = new Label { Text = "Talep Tipi:", Location = new Point(20, y), AutoSize = true };
            _cmbTip = new ComboBox { Location = new Point(110, y - 3), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbTip.DataSource = Enum.GetValues(typeof(TalepTipi));

            _chkEkap = new CheckBox { Text = "EKAP Yapıldı mı?", Location = new Point(350, y), AutoSize = true };

            grpBaslik.Controls.AddRange(new Control[]
            {
                lblBirim, _cmbBirim,
                lblNo, _txtTalepNo,
                lblTalepEden, _cmbTalepEden,
                lblTarih, _dtpTarih,
                lblSevk, _dtpSevkTarihi,
                lblTip, _cmbTip,
                _chkEkap
            });

            this.Controls.Add(grpBaslik);

            // -----------------------------------
            // ÜRÜN EKLEME BÖLÜMÜ
            // -----------------------------------

            GroupBox grpDetay = new GroupBox { Text = "Malzeme Listesi Oluştur", Location = new Point(10, 180), Size = new Size(960, 100) };

            Label lblUrun = new Label { Text = "Stok Kartı:", Location = new Point(20, 35), AutoSize = true };
            _txtSecilenUrunAdi = new TextBox { Location = new Point(90, 32), Width = 380, ReadOnly = true, BackColor = Color.WhiteSmoke };

            Button btnBul = new Button { Text = "BUL", Location = new Point(475, 30), Width = 50, Height = 26, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnBul.Click += BtnBul_Click;

            Label lblMiktar = new Label { Text = "Miktar:", Location = new Point(540, 35), AutoSize = true };
            _numMiktar = new NumericUpDown { Location = new Point(590, 32), Width = 80, Maximum = 10000 };

            _cmbOlcu = new ComboBox { Location = new Point(680, 32), Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };

            _btnListeyeEkle = new Button { Text = "EKLE", Location = new Point(780, 30), Width = 80, BackColor = Color.Orange };
            _btnListeyeEkle.Click += BtnListeyeEkle_Click;

            _btnSatirSil = new Button { Text = "SATIR SİL", Location = new Point(870, 30), Width = 80, BackColor = Color.IndianRed, ForeColor = Color.White };
            _btnSatirSil.Click += (s, e) =>
            {
                if (_gridSepet.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in _gridSepet.SelectedRows)
                        if (!row.IsNewRow) _gridSepet.Rows.Remove(row);
                }
            };

            grpDetay.Controls.AddRange(new Control[]
            {
                lblUrun, _txtSecilenUrunAdi, btnBul, lblMiktar, _numMiktar, _cmbOlcu, _btnListeyeEkle, _btnSatirSil
            });

            this.Controls.Add(grpDetay);

            _gridSepet = new DataGridView
            {
                Location = new Point(10, 290),
                Size = new Size(960, 320),
                DataSource = _dtSepet,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(_gridSepet);

            _btnKaydet = new Button
            {
                Text = "TALEBİ TAMAMLA VE KAYDET",
                Location = new Point(690, 620),
                Size = new Size(270, 50),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            _btnKaydet.Click += BtnKaydet_Click;

            this.Controls.Add(_btnKaydet);
        }

        private void VerileriYukle()
        {
            DataTable dtBirim = Veritabani.VeriGetir("SELECT BirimAdi FROM Birimler");
            foreach (DataRow row in dtBirim.Rows)
                _cmbBirim.Items.Add(row["BirimAdi"].ToString());

            DataTable dtOlcu = Veritabani.VeriGetir("SELECT BirimAdi FROM OlcuBirimleri");
            foreach (DataRow row in dtOlcu.Rows)
                _cmbOlcu.Items.Add(row["BirimAdi"].ToString());

            if (_cmbOlcu.Items.Count > 0)
                _cmbOlcu.SelectedIndex = 0;
        }

        private void EskiVerileriGetir()
        {
            DataRow baslik = Veritabani.VeriGetir($"SELECT * FROM TalepBaslik WHERE Id={_duzenlenenId}").Rows[0];

            _txtTalepNo.Text = baslik["TalepNo"].ToString();
            _cmbBirim.SelectedItem = baslik["TalepBirimi"].ToString();
            _cmbTip.SelectedItem = Enum.Parse(typeof(TalepTipi), baslik["Tip"].ToString());
            _dtpTarih.Value = Convert.ToDateTime(baslik["TalepTarihi"]);
            _dtpSevkTarihi.Value = Convert.ToDateTime(baslik["SevkTarihi"]);
            _chkEkap.Checked = Convert.ToInt32(baslik["EkapYapildi"]) == 1;

            // Talep eden
            string eden = baslik["TalepEden"]?.ToString() ?? _talepEden;
            _cmbTalepEden.SelectedItem = eden;

            DataTable detaylar = Veritabani.VeriGetir($"SELECT UrunId, UrunAdi, Miktar, Birim FROM TalepDetay WHERE TalepBaslikId={_duzenlenenId}");
            foreach (DataRow dbRow in detaylar.Rows)
            {
                DataRow row = _dtSepet.NewRow();
                row["UrunId"] = dbRow["UrunId"];
                row["UrunAdi"] = dbRow["UrunAdi"];
                row["Miktar"] = dbRow["Miktar"];
                row["Birim"] = dbRow["Birim"];
                _dtSepet.Rows.Add(row);
            }
        }

        private void BtnBul_Click(object? sender, EventArgs e)
        {
            StokBulForm aramaFormu = new StokBulForm();
            if (aramaFormu.ShowDialog() == DialogResult.OK)
            {
                _secilenUrunId = aramaFormu.SecilenUrunId;
                _txtSecilenUrunAdi.Text = aramaFormu.SecilenUrunTamAdi;
                _numMiktar.Focus();
            }
        }

        private void BtnListeyeEkle_Click(object? sender, EventArgs e)
        {
            if (_secilenUrunId == 0 || _numMiktar.Value <= 0)
            {
                MessageBox.Show("Lütfen ürün seçin ve miktar girin.");
                return;
            }

            DataRow row = _dtSepet.NewRow();
            row["UrunId"] = _secilenUrunId;
            row["UrunAdi"] = _txtSecilenUrunAdi.Text;
            row["Miktar"] = _numMiktar.Value;
            row["Birim"] = _cmbOlcu.Text;
            _dtSepet.Rows.Add(row);

            _numMiktar.Value = 0;
            _secilenUrunId = 0;
            _txtSecilenUrunAdi.Clear();
        }

        private void BtnKaydet_Click(object? sender, EventArgs e)
        {
            if (_dtSepet.Rows.Count == 0 || _cmbBirim.SelectedIndex == -1)
            {
                MessageBox.Show("Eksik bilgi!");
                return;
            }

            string secilenTalepEden = _cmbTalepEden.SelectedItem?.ToString() ?? "";

            if (_duzenlenenId == 0)
            {
                // Yeni kayıt
                string sqlBaslik = $@"
INSERT INTO TalepBaslik 
(TalepNo, TalepTarihi, SevkTarihi, TalepBirimi, Tip, EkapYapildi, Durum, TalepEden)
VALUES 
('{_txtTalepNo.Text}', 
 '{_dtpTarih.Value:yyyy-MM-dd}', 
 '{_dtpSevkTarihi.Value:yyyy-MM-dd}', 
 '{_cmbBirim.Text}', 
 '{_cmbTip.Text}', 
 {(_chkEkap.Checked ? 1 : 0)}, 
 'Acik',
 '{secilenTalepEden}'
)";
                Veritabani.SorguCalistir(sqlBaslik);

                object idObj = Veritabani.TekDegerGetir($"SELECT Id FROM TalepBaslik WHERE TalepNo='{_txtTalepNo.Text}'");
                int yeniId = Convert.ToInt32(idObj);

                foreach (DataRow row in _dtSepet.Rows)
                {
                    string sqlDetay =
                        $@"INSERT INTO TalepDetay (TalepBaslikId, UrunId, UrunAdi, Miktar, Birim)
                           VALUES ({yeniId}, {row["UrunId"]}, '{row["UrunAdi"]}', {row["Miktar"]}, '{row["Birim"]}')";
                    Veritabani.SorguCalistir(sqlDetay);
                }

                MessageBox.Show("Talep başarıyla eklendi.");
            }
            else
            {
                // Güncelleme
                string sqlBaslik = $@"
UPDATE TalepBaslik SET 
TalepTarihi='{_dtpTarih.Value:yyyy-MM-dd}',
SevkTarihi='{_dtpSevkTarihi.Value:yyyy-MM-dd}',
TalepBirimi='{_cmbBirim.Text}',
Tip='{_cmbTip.Text}',
EkapYapildi={(_chkEkap.Checked ? 1 : 0)},
TalepEden='{secilenTalepEden}'
WHERE Id={_duzenlenenId}";
                Veritabani.SorguCalistir(sqlBaslik);

                Veritabani.SorguCalistir($"DELETE FROM TalepDetay WHERE TalepBaslikId={_duzenlenenId}");

                foreach (DataRow row in _dtSepet.Rows)
                {
                    string sqlDetay =
                        $@"INSERT INTO TalepDetay (TalepBaslikId, UrunId, UrunAdi, Miktar, Birim)
                           VALUES ({_duzenlenenId}, {row["UrunId"]}, '{row["UrunAdi"]}', {row["Miktar"]}, '{row["Birim"]}')";
                    Veritabani.SorguCalistir(sqlDetay);
                }

                MessageBox.Show("Talep başarıyla güncellendi.");
            }

            this.Close();
        }
    }
}
