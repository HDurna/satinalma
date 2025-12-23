using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using satinalma.Data;

namespace satinalma.Forms
{
    public class TanimlamalarForm : Form
    {
        private TabControl _tabAna;

        private string _seciliPersonelId = "0";
        private string _seciliUrunId = "0";
        private string _seciliOlcuId = "0";
        private string _seciliBirimId = "0";
        private string _seciliKategoriId = "0";

        private ComboBox _cmbBirimYetkili;
        private ComboBox _cmbUrunKategori;

        public TanimlamalarForm()
        {
            // ORTAK TEMA
            FormGorunum.Uygula(this);

            Text = "Sistem Tanımlamaları ve Yönetim";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            _tabAna = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            _tabAna.SelectedIndexChanged += (s, e) =>
            {
                try
                {
                    if (_tabAna.SelectedTab.Text == "İşletme Birimleri")
                        ComboDoldur(_cmbBirimYetkili, "Kullanicilar", "AdSoyad");

                    if (_tabAna.SelectedTab.Text == "Stok Kartları")
                        ComboDoldur(_cmbUrunKategori, "Kategoriler", "KategoriAdi");
                }
                catch { }
            };

            Controls.Add(_tabAna);

            SekmeOlustur_Birimler();
            SekmeOlustur_Kullanicilar();
            SekmeOlustur_Kategoriler();
            SekmeOlustur_Urunler();
            SekmeOlustur_OlcuBirimleri();
            SekmeOlustur_Tedarikciler();
        }

        // -------------------------------------------------------------
        //  ORTAK YARDIMCILAR
        // -------------------------------------------------------------
        private void ComboDoldur(ComboBox cmb, string tablo, string kolon)
        {
            if (cmb == null) return;

            cmb.Items.Clear();
            DataTable dt = Veritabani.VeriGetir($"SELECT {kolon} FROM {tablo}");
            if (dt == null) return;

            foreach (DataRow row in dt.Rows)
            {
                var value = row[kolon]?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                    cmb.Items.Add(value);
            }
        }

        private Button YeniButon(string text, Color backColor, Color? foreColor = null)
        {
            return new Button
            {
                Text = text,
                Width = 100,
                Height = 32,
                BackColor = backColor,
                ForeColor = foreColor ?? Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4, 0, 4, 0)
            };
        }

        private FlowLayoutPanel CreateButtonBar(
            out Button btnKaydet,
            out Button btnGuncelle,
            out Button btnSil,
            out Button btnTemizle)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            btnKaydet = YeniButon("KAYDET", Color.SteelBlue);
            btnGuncelle = YeniButon("GÜNCELLE", Color.Goldenrod);
            btnSil = YeniButon("SİL", Color.IndianRed);
            btnTemizle = YeniButon("TEMİZLE", Color.WhiteSmoke, Color.Black);

            panel.Controls.AddRange(new Control[] { btnKaydet, btnGuncelle, btnSil, btnTemizle });

            return panel;
        }

        private DataGridView YeniGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                AllowUserToAddRows = false
            };
        }

        /// <summary>
        /// Sekme iskeleti: üstte header panel, altta grid – hepsi Dock/Anchor ile.
        /// </summary>
        private (TableLayoutPanel root, Panel header, DataGridView grid) YeniSekmeIskeleti(TabPage page)
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // header
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));      // grid

            var header = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            var grid = YeniGrid();

            root.Controls.Add(header, 0, 0);
            root.Controls.Add(grid, 0, 1);

            page.Controls.Add(root);

            return (root, header, grid);
        }

        // -------------------------------------------------------------
        //  İŞLETME BİRİMLERİ
        // -------------------------------------------------------------
        private void SekmeOlustur_Birimler()
        {
            var page = new TabPage("İşletme Birimleri");
            var (root, header, grid) = YeniSekmeIskeleti(page);

            // Alanlar: TableLayoutPanel 2x2
            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 2,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));

            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblBirim = new Label { Text = "Birim Adı:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtBirim = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 0, 3), Width = 230 };

            var lblYetkili = new Label { Text = "Harcama Yetkilisi:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            _cmbBirimYetkili = new ComboBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 3, 0, 3),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 230
            };

            fields.Controls.Add(lblBirim, 0, 0);
            fields.Controls.Add(txtBirim, 1, 0);
            fields.Controls.Add(lblYetkili, 0, 1);
            fields.Controls.Add(_cmbBirimYetkili, 1, 1);

            // Buton barı
            Button btnKaydet, btnGuncelle, btnSil, btnTemizle;
            var btnPanel = CreateButtonBar(out btnKaydet, out btnGuncelle, out btnSil, out btnTemizle);

            header.Controls.Add(btnPanel);
            header.Controls.Add(fields);

            _tabAna.TabPages.Add(page);

            // --- İş mantığı ---
            void Listele()
            {
                grid.DataSource = Veritabani.VeriGetir("SELECT * FROM Birimler ORDER BY Id DESC");
            }

            void Temizle()
            {
                txtBirim.Clear();
                _cmbBirimYetkili.SelectedIndex = -1;
                _seciliBirimId = "0";
            }

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                _seciliBirimId = grid.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                txtBirim.Text = grid.Rows[e.RowIndex].Cells["BirimAdi"].Value.ToString();

                if (grid.Columns.Contains("HarcamaYetkilisi"))
                {
                    var yetkili = grid.Rows[e.RowIndex].Cells["HarcamaYetkilisi"].Value?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(yetkili))
                    {
                        if (!_cmbBirimYetkili.Items.Contains(yetkili))
                            _cmbBirimYetkili.Items.Add(yetkili);

                        _cmbBirimYetkili.SelectedItem = yetkili;
                    }
                    else
                    {
                        _cmbBirimYetkili.SelectedIndex = -1;
                    }
                }
            };

            btnKaydet.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBirim.Text))
                {
                    MessageBox.Show("Birim adı boş olamaz.");
                    return;
                }

                string yetkili = _cmbBirimYetkili.SelectedItem?.ToString() ?? "";
                Veritabani.SorguCalistir(
                    $"INSERT INTO Birimler (BirimAdi, HarcamaYetkilisi) VALUES ('{txtBirim.Text}', '{yetkili}')");

                Listele();
                Temizle();
            };

            btnGuncelle.Click += (s, e) =>
            {
                if (_seciliBirimId == "0") return;

                string yetkili = _cmbBirimYetkili.SelectedItem?.ToString() ?? "";
                Veritabani.SorguCalistir(
                    $"UPDATE Birimler SET BirimAdi='{txtBirim.Text}', HarcamaYetkilisi='{yetkili}' WHERE Id={_seciliBirimId}");

                Listele();
                Temizle();
            };

            btnSil.Click += (s, e) =>
            {
                if (_seciliBirimId == "0") return;

                if (MessageBox.Show("Seçili birimi silmek istiyor musunuz?",
                                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Veritabani.SorguCalistir($"DELETE FROM Birimler WHERE Id={_seciliBirimId}");
                    Listele();
                    Temizle();
                }
            };

            btnTemizle.Click += (s, e) => Temizle();

            Listele();
            ComboDoldur(_cmbBirimYetkili, "Kullanicilar", "AdSoyad");
        }

        // -------------------------------------------------------------
        //  PERSONEL
        // -------------------------------------------------------------
        private void SekmeOlustur_Kullanicilar()
        {
            var page = new TabPage("Personel");
            var (_, header, grid) = YeniSekmeIskeleti(page);

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 4,
                RowCount = 2,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));          // label 1
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));     // textbox 1
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));          // label 2
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));     // textbox 2

            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblAd = new Label { Text = "Ad Soyad:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtAd = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 20, 3), Width = 200 };

            var lblSifre = new Label { Text = "Şifre:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtSifre = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 0, 3), Width = 200 };

            var lblKadi = new Label { Text = "Kullanıcı Adı:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtKadi = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 20, 3), Width = 200 };

            var lblYetki = new Label { Text = "Yetki:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var cmbYetki = new ComboBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 3, 0, 3),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200
            };
            cmbYetki.Items.AddRange(new object[] { "Admin", "SatinalmaPersoneli", "BirimPersoneli" });

            fields.Controls.Add(lblAd, 0, 0);
            fields.Controls.Add(txtAd, 1, 0);
            fields.Controls.Add(lblSifre, 2, 0);
            fields.Controls.Add(txtSifre, 3, 0);

            fields.Controls.Add(lblKadi, 0, 1);
            fields.Controls.Add(txtKadi, 1, 1);
            fields.Controls.Add(lblYetki, 2, 1);
            fields.Controls.Add(cmbYetki, 3, 1);

            Button btnKaydet, btnGuncelle, btnSil, btnTemizle;
            var btnPanel = CreateButtonBar(out btnKaydet, out btnGuncelle, out btnSil, out btnTemizle);

            header.Controls.Add(btnPanel);
            header.Controls.Add(fields);

            _tabAna.TabPages.Add(page);

            void Listele()
            {
                grid.DataSource = Veritabani.VeriGetir("SELECT Id, AdSoyad, KullaniciAdi, Yetki FROM Kullanicilar ORDER BY Id DESC");
            }

            void Temizle()
            {
                txtAd.Clear();
                txtKadi.Clear();
                txtSifre.Clear();
                cmbYetki.SelectedIndex = -1;
                _seciliPersonelId = "0";
            }

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                _seciliPersonelId = grid.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                txtAd.Text = grid.Rows[e.RowIndex].Cells["AdSoyad"].Value.ToString();
                txtKadi.Text = grid.Rows[e.RowIndex].Cells["KullaniciAdi"].Value.ToString();
                cmbYetki.SelectedItem = grid.Rows[e.RowIndex].Cells["Yetki"].Value.ToString();

                object sifreObj = Veritabani.TekDegerGetir($"SELECT Sifre FROM Kullanicilar WHERE Id={_seciliPersonelId}");
                txtSifre.Text = sifreObj != null && sifreObj != DBNull.Value ? sifreObj.ToString() : "";
            };

            btnKaydet.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtKadi.Text) || cmbYetki.SelectedIndex == -1)
                {
                    MessageBox.Show("Kullanıcı adı ve yetki zorunlu.");
                    return;
                }

                Veritabani.SorguCalistir(
                    $"INSERT INTO Kullanicilar (AdSoyad, KullaniciAdi, Sifre, Yetki) " +
                    $"VALUES ('{txtAd.Text}', '{txtKadi.Text}', '{txtSifre.Text}', '{cmbYetki.SelectedItem}')");

                Listele();
                Temizle();
            };

            btnGuncelle.Click += (s, e) =>
            {
                if (_seciliPersonelId == "0") return;

                Veritabani.SorguCalistir(
                    $"UPDATE Kullanicilar SET " +
                    $"AdSoyad='{txtAd.Text}', " +
                    $"KullaniciAdi='{txtKadi.Text}', " +
                    $"Sifre='{txtSifre.Text}', " +
                    $"Yetki='{cmbYetki.SelectedItem}' " +
                    $"WHERE Id={_seciliPersonelId}");

                Listele();
                Temizle();
            };

            btnSil.Click += (s, e) =>
            {
                if (_seciliPersonelId == "0") return;

                if (MessageBox.Show("Seçili kullanıcıyı silmek istiyor musunuz?",
                                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Veritabani.SorguCalistir($"DELETE FROM Kullanicilar WHERE Id={_seciliPersonelId}");
                    Listele();
                    Temizle();
                }
            };

            btnTemizle.Click += (s, e) => Temizle();

            Listele();
        }

        // -------------------------------------------------------------
        //  KATEGORİLER
        // -------------------------------------------------------------
        private void SekmeOlustur_Kategoriler()
        {
            var page = new TabPage("Kategoriler");
            var (_, header, grid) = YeniSekmeIskeleti(page);

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblAd = new Label { Text = "Kategori Adı:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtAd = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 0, 3), Width = 230 };

            fields.Controls.Add(lblAd, 0, 0);
            fields.Controls.Add(txtAd, 1, 0);

            Button btnKaydet, btnGuncelle, btnSil, btnTemizle;
            var btnPanel = CreateButtonBar(out btnKaydet, out btnGuncelle, out btnSil, out btnTemizle);

            header.Controls.Add(btnPanel);
            header.Controls.Add(fields);

            _tabAna.TabPages.Add(page);

            void Listele()
            {
                grid.DataSource = Veritabani.VeriGetir("SELECT * FROM Kategoriler ORDER BY Id DESC");
            }

            void Temizle()
            {
                txtAd.Clear();
                _seciliKategoriId = "0";
            }

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                _seciliKategoriId = grid.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                txtAd.Text = grid.Rows[e.RowIndex].Cells["KategoriAdi"].Value.ToString();
            };

            btnKaydet.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtAd.Text)) return;

                Veritabani.SorguCalistir(
                    $"INSERT INTO Kategoriler (KategoriAdi) VALUES ('{txtAd.Text}')");

                Listele();
                Temizle();
            };

            btnGuncelle.Click += (s, e) =>
            {
                if (_seciliKategoriId == "0") return;

                Veritabani.SorguCalistir(
                    $"UPDATE Kategoriler SET KategoriAdi='{txtAd.Text}' WHERE Id={_seciliKategoriId}");

                Listele();
                Temizle();
            };

            btnSil.Click += (s, e) =>
            {
                if (_seciliKategoriId == "0") return;

                if (MessageBox.Show("Seçili kategoriyi silmek istiyor musunuz?",
                                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Veritabani.SorguCalistir($"DELETE FROM Kategoriler WHERE Id={_seciliKategoriId}");
                    Listele();
                    Temizle();
                }
            };

            btnTemizle.Click += (s, e) => Temizle();

            Listele();
        }

        // -------------------------------------------------------------
        //  ÖLÇÜ BİRİMLERİ
        // -------------------------------------------------------------
        private void SekmeOlustur_OlcuBirimleri()
        {
            var page = new TabPage("Ölçü Birimleri");
            var (_, header, grid) = YeniSekmeIskeleti(page);

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblAd = new Label { Text = "Birim:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtAd = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 0, 3), Width = 170 };

            fields.Controls.Add(lblAd, 0, 0);
            fields.Controls.Add(txtAd, 1, 0);

            Button btnKaydet, btnGuncelle, btnSil, btnTemizle;
            var btnPanel = CreateButtonBar(out btnKaydet, out btnGuncelle, out btnSil, out btnTemizle);

            header.Controls.Add(btnPanel);
            header.Controls.Add(fields);

            _tabAna.TabPages.Add(page);

            void Listele()
            {
                grid.DataSource = Veritabani.VeriGetir("SELECT * FROM OlcuBirimleri ORDER BY Id DESC");
            }

            void Temizle()
            {
                txtAd.Clear();
                _seciliOlcuId = "0";
            }

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                _seciliOlcuId = grid.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                txtAd.Text = grid.Rows[e.RowIndex].Cells["BirimAdi"].Value.ToString();
            };

            btnKaydet.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtAd.Text)) return;

                Veritabani.SorguCalistir(
                    $"INSERT INTO OlcuBirimleri (BirimAdi) VALUES ('{txtAd.Text}')");

                Listele();
                Temizle();
            };

            btnGuncelle.Click += (s, e) =>
            {
                if (_seciliOlcuId == "0") return;

                Veritabani.SorguCalistir(
                    $"UPDATE OlcuBirimleri SET BirimAdi='{txtAd.Text}' WHERE Id={_seciliOlcuId}");

                Listele();
                Temizle();
            };

            btnSil.Click += (s, e) =>
            {
                if (_seciliOlcuId == "0") return;

                if (MessageBox.Show("Seçili ölçü birimini silmek istiyor musunuz?",
                                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Veritabani.SorguCalistir($"DELETE FROM OlcuBirimleri WHERE Id={_seciliOlcuId}");
                    Listele();
                    Temizle();
                }
            };

            btnTemizle.Click += (s, e) => Temizle();

            Listele();
        }

        // -------------------------------------------------------------
        //  STOK KARTLARI
        // -------------------------------------------------------------
        private void SekmeOlustur_Urunler()
        {
            var page = new TabPage("Stok Kartları");
            var (_, header, grid) = YeniSekmeIskeleti(page);

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 6,
                RowCount = 2,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            // sütun: label / field çiftleri
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));        // lbl1
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));  // fld1
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));        // lbl2
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));  // fld2
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));        // lbl3
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));  // fld3

            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblKat = new Label { Text = "Kategori:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            _cmbUrunKategori = new ComboBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 3, 20, 3),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 160
            };

            var lblUrun = new Label { Text = "Ürün Adı:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtUrun = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 20, 3), Width = 160 };

            var lblOz1 = new Label { Text = "Özellik 1:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtOz1 = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 0, 3), Width = 160 };

            var lblOz2 = new Label { Text = "Özellik 2:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtOz2 = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 20, 3), Width = 160 };

            var lblOz3 = new Label { Text = "Özellik 3:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 4, 6) };
            var txtOz3 = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 0, 3), Width = 160 };

            // Satır 0: Kategori - Ürün
            fields.Controls.Add(lblKat, 0, 0);
            fields.Controls.Add(_cmbUrunKategori, 1, 0);
            fields.Controls.Add(lblUrun, 2, 0);
            fields.Controls.Add(txtUrun, 3, 0);

            // Satır 1: Özellikler
            fields.Controls.Add(lblOz1, 0, 1);
            fields.Controls.Add(txtOz1, 1, 1);
            fields.Controls.Add(lblOz2, 2, 1);
            fields.Controls.Add(txtOz2, 3, 1);
            fields.Controls.Add(lblOz3, 4, 1);
            fields.Controls.Add(txtOz3, 5, 1);

            Button btnKaydet, btnGuncelle, btnSil, btnTemizle;
            var btnPanel = CreateButtonBar(out btnKaydet, out btnGuncelle, out btnSil, out btnTemizle);

            header.Controls.Add(btnPanel);
            header.Controls.Add(fields);

            _tabAna.TabPages.Add(page);

            string IsimOlustur()
            {
                string sonuc = $"{_cmbUrunKategori.SelectedItem} - {txtUrun.Text}";
                if (!string.IsNullOrWhiteSpace(txtOz1.Text)) sonuc += $" - {txtOz1.Text}";
                if (!string.IsNullOrWhiteSpace(txtOz2.Text)) sonuc += $" - {txtOz2.Text}";
                if (!string.IsNullOrWhiteSpace(txtOz3.Text)) sonuc += $" - {txtOz3.Text}";
                return sonuc;
            }

            void Listele()
            {
                grid.AutoGenerateColumns = true;
                grid.DataSource = Veritabani.VeriGetir("SELECT * FROM Urunler ORDER BY Id DESC");
            }

            void Temizle()
            {
                txtUrun.Clear();
                txtOz1.Clear();
                txtOz2.Clear();
                txtOz3.Clear();
                _cmbUrunKategori.SelectedIndex = -1;
                _seciliUrunId = "0";
            }

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                _seciliUrunId = grid.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                _cmbUrunKategori.SelectedItem = grid.Rows[e.RowIndex].Cells["KategoriAdi"].Value.ToString();
                txtUrun.Text = grid.Rows[e.RowIndex].Cells["UrunAdi"].Value.ToString();
                txtOz1.Text = grid.Rows[e.RowIndex].Cells["Ozellik1"].Value.ToString();
                txtOz2.Text = grid.Rows[e.RowIndex].Cells["Ozellik2"].Value.ToString();
                txtOz3.Text = grid.Rows[e.RowIndex].Cells["Ozellik3"].Value.ToString();
            };

            btnKaydet.Click += (s, e) =>
            {
                if (_cmbUrunKategori.SelectedItem == null || string.IsNullOrWhiteSpace(txtUrun.Text))
                {
                    MessageBox.Show("Kategori ve ürün adı zorunlu.");
                    return;
                }

                string tamAd = IsimOlustur();

                Veritabani.SorguCalistir(
                    $"INSERT INTO Urunler (KategoriAdi, UrunAdi, Ozellik1, Ozellik2, Ozellik3, TamAd) " +
                    $"VALUES ('{_cmbUrunKategori.SelectedItem}', '{txtUrun.Text}', '{txtOz1.Text}', '{txtOz2.Text}', '{txtOz3.Text}', '{tamAd}')");

                Listele();
                Temizle();
            };

            btnGuncelle.Click += (s, e) =>
            {
                if (_seciliUrunId == "0") return;

                string tamAd = IsimOlustur();

                Veritabani.SorguCalistir(
                    $"UPDATE Urunler SET " +
                    $"KategoriAdi='{_cmbUrunKategori.SelectedItem}', " +
                    $"UrunAdi='{txtUrun.Text}', " +
                    $"Ozellik1='{txtOz1.Text}', " +
                    $"Ozellik2='{txtOz2.Text}', " +
                    $"Ozellik3='{txtOz3.Text}', " +
                    $"TamAd='{tamAd}' " +
                    $"WHERE Id={_seciliUrunId}");

                Listele();
                Temizle();
            };

            btnSil.Click += (s, e) =>
            {
                if (_seciliUrunId == "0") return;

                if (MessageBox.Show("Seçili stok kartını silmek istiyor musunuz?",
                                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Veritabani.SorguCalistir($"DELETE FROM Urunler WHERE Id={_seciliUrunId}");
                    Listele();
                    Temizle();
                }
            };

            btnTemizle.Click += (s, e) => Temizle();

            Listele();
            ComboDoldur(_cmbUrunKategori, "Kategoriler", "KategoriAdi");
        }

        // ==================== TEDARİKÇİLER SEKMESİ ====================
        private void SekmeOlustur_Tedarikciler()
        {
            var tabPage = new TabPage("Tedarikçiler") { Padding = new Padding(10) };
            _tabAna.TabPages.Add(tabPage);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            // Grid
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Form alanları
            var formPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            Label lblFirma = new Label { Text = "Firma Adı:", Location = new Point(20, 20), AutoSize = true };
            var txtFirmaAdi = new TextBox { Location = new Point(150, 17), Width = 250 };

            Label lblTip = new Label { Text = "Firma Tipi:", Location = new Point(20, 55), AutoSize = true };
            var cmbTip = new ComboBox { Location = new Point(150, 52), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTip.Items.AddRange(new[] { "Ticari", "Şahıs" });
            cmbTip.SelectedIndex = 0;

            Label lblVergi = new Label { Text = "Vergi No:", Location = new Point(20, 90), AutoSize = true };
            var txtVergiNo = new TextBox { Location = new Point(150, 87), Width = 150 };

            Label lblDogum = new Label { Text = "Doğum Tarihi:", Location = new Point(20, 125), AutoSize = true, Enabled = false };
            var dtpDogum = new DateTimePicker { Location = new Point(150, 122), Width = 150, Enabled = false };

            // Firma tipi değişince doğum tarihi aktif/pasif
            cmbTip.SelectedIndexChanged += (s, e) =>
            {
                bool sahis = cmbTip.SelectedItem?.ToString() == "Şahıs";
                lblDogum.Enabled = sahis;
                dtpDogum.Enabled = sahis;
            };

            Label lblAdres = new Label { Text = "Adres:", Location = new Point(20, 160), AutoSize = true };
            var txtAdres = new TextBox { Location = new Point(150, 157), Width = 300, Height = 60, Multiline = true };

            Label lblTel = new Label { Text = "Telefon:", Location = new Point(480, 20), AutoSize = true };
            var txtTelefon = new TextBox { Location = new Point(570, 17), Width = 150 };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(480, 55), AutoSize = true };
            var txtEmail = new TextBox { Location = new Point(570, 52), Width = 200 };

            Label lblYetkili = new Label { Text = "Yetkili Kişi:", Location = new Point(480, 90), AutoSize = true };
            var txtYetkili = new TextBox { Location = new Point(570, 87), Width = 200 };

            var chkAktif = new CheckBox { Text = "Aktif", Location = new Point(570, 125), Checked = true };

            // Butonlar
            Button btnKaydet, btnGuncelle, btnSil, btnTemizle;
            var buttonBar = CreateButtonBar(out btnKaydet, out btnGuncelle, out btnSil, out btnTemizle);
            buttonBar.Location = new Point(20, 230);

            int seciliId = 0;

            Action Listele = () =>
            {
                using (var context = new SatinAlmaDbContext())
                {
                    var list = context.TedarikciFirmalar.Select(t => new
                    {
                        t.Id,
                        t.FirmaAdi,
                        t.FirmaTipi,
                        t.VergiNo,
                        t.Telefon,
                        t.Email,
                        t.YetkiliKisi,
                        Durum = t.Aktif ? "Aktif" : "Pasif"
                    }).ToList();
                    grid.DataSource = list;
                }
            };

            Action Temizle = () =>
            {
                seciliId = 0;
                txtFirmaAdi.Clear();
                cmbTip.SelectedIndex = 0;
                txtVergiNo.Clear();
                dtpDogum.Value = DateTime.Now;
                txtAdres.Clear();
                txtTelefon.Clear();
                txtEmail.Clear();
                txtYetkili.Clear();
                chkAktif.Checked = true;
            };

            grid.SelectionChanged += (s, e) =>
            {
                if (grid.SelectedRows.Count > 0)
                {
                    seciliId = Convert.ToInt32(grid.SelectedRows[0].Cells["Id"].Value);
                    using (var context = new SatinAlmaDbContext())
                    {
                        var firma = context.TedarikciFirmalar.Find(seciliId);
                        if (firma != null)
                        {
                            txtFirmaAdi.Text = firma.FirmaAdi;
                            cmbTip.SelectedItem = firma.FirmaTipi;
                            txtVergiNo.Text = firma.VergiNo;
                            if (firma.DogumTarihi.HasValue)
                                dtpDogum.Value = firma.DogumTarihi.Value;
                            txtAdres.Text = firma.Adres;
                            txtTelefon.Text = firma.Telefon;
                            txtEmail.Text = firma.Email;
                            txtYetkili.Text = firma.YetkiliKisi;
                            chkAktif.Checked = firma.Aktif;
                        }
                    }
                }
            };

            btnKaydet.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtFirmaAdi.Text))
                {
                    MessageBox.Show("Firma adı boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new SatinAlmaDbContext())
                {
                    var yeni = new satinalma.Models.Entities.TedarikçiFirma
                    {
                        FirmaAdi = txtFirmaAdi.Text,
                        FirmaTipi = cmbTip.SelectedItem?.ToString() ?? "Ticari",
                        VergiNo = txtVergiNo.Text,
                        DogumTarihi = cmbTip.SelectedItem?.ToString() == "Şahıs" ? dtpDogum.Value : (DateTime?)null,
                        Adres = txtAdres.Text,
                        Telefon = txtTelefon.Text,
                        Email = txtEmail.Text,
                        YetkiliKisi = txtYetkili.Text,
                        Aktif = chkAktif.Checked
                    };

                    context.TedarikciFirmalar.Add(yeni);
                    context.SaveChanges();
                    MessageBox.Show("Tedarikçi kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Listele();
                    Temizle();
                }
            };

            btnGuncelle.Click += (s, e) =>
            {
                if (seciliId == 0)
                {
                    MessageBox.Show("Lütfen güncellemek için bir tedarikçi seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new SatinAlmaDbContext())
                {
                    var firma = context.TedarikciFirmalar.Find(seciliId);
                    if (firma != null)
                    {
                        firma.FirmaAdi = txtFirmaAdi.Text;
                        firma.FirmaTipi = cmbTip.SelectedItem?.ToString() ?? "Ticari";
                        firma.VergiNo = txtVergiNo.Text;
                        firma.DogumTarihi = cmbTip.SelectedItem?.ToString() == "Şahıs" ? dtpDogum.Value : (DateTime?)null;
                        firma.Adres = txtAdres.Text;
                        firma.Telefon = txtTelefon.Text;
                        firma.Email = txtEmail.Text;
                        firma.YetkiliKisi = txtYetkili.Text;
                        firma.Aktif = chkAktif.Checked;

                        context.SaveChanges();
                        MessageBox.Show("Tedarikçi güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Listele();
                        Temizle();
                    }
                }
            };

            btnSil.Click += (s, e) =>
            {
                if (seciliId == 0)
                {
                    MessageBox.Show("Lütfen silmek için bir tedarikçi seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Bu tedarikçiyi silmek istediğinizden emin misiniz?", "Onay",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    using (var context = new SatinAlmaDbContext())
                    {
                        var firma = context.TedarikciFirmalar.Find(seciliId);
                        if (firma != null)
                        {
                            context.TedarikciFirmalar.Remove(firma);
                            context.SaveChanges();
                            MessageBox.Show("Tedarikçi silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Listele();
                            Temizle();
                        }
                    }
                }
            };

            btnTemizle.Click += (s, e) => Temizle();

            formPanel.Controls.AddRange(new Control[] {
                lblFirma, txtFirmaAdi, lblTip, cmbTip, lblVergi, txtVergiNo,
                lblDogum, dtpDogum, lblAdres, txtAdres, lblTel, txtTelefon,
                lblEmail, txtEmail, lblYetkili, txtYetkili, chkAktif, buttonBar
            });

            mainLayout.Controls.Add(grid, 0, 0);
            mainLayout.Controls.Add(formPanel, 0, 1);
            tabPage.Controls.Add(mainLayout);

            Listele();
        }
    }
}
