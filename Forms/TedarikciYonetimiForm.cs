using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using satinalma.Data;
using satinalma.Models.Entities;

namespace satinalma.Forms
{
    public class TedarikciYonetimiForm : Form
    {
        private DataGridView _grid = null!;
        private Button _btnYeni, _btnDuzelt, _btnSil = null!;
        private TextBox _txtFirmaAdi, _txtVergiNo, _txtAdres, _txtTelefon, _txtEmail, _txtYetkili = null!;
        private CheckBox _chkAktif = null!;
        private int _seciliId = 0;

        public TedarikciYonetimiForm()
        {
            FormGorunum.Uygula(this);
            this.Text = "Tedarikçi Firma Yönetimi";
            BilesenleriOlustur();
            ListeyiYukle();
        }

        private void BilesenleriOlustur()
        {
            // Grid
            _grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(800, 400),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _grid.SelectionChanged += Grid_SelectionChanged;

            // Input fields
            Label lblFirma = new Label { Text = "Firma Adı:", Location = new Point(20, 440), AutoSize = true };
            _txtFirmaAdi = new TextBox { Location = new Point(120, 437), Width = 200 };

            Label lblVergi = new Label { Text = "Vergi No:", Location = new Point(20, 470), AutoSize = true };
            _txtVergiNo = new TextBox { Location = new Point(120, 467), Width = 150 };

            Label lblAdres = new Label { Text = "Adres:", Location = new Point(20, 500), AutoSize = true };
            _txtAdres = new TextBox { Location = new Point(120, 497), Width = 300, Height = 60, Multiline = true };

            Label lblTel = new Label { Text = "Telefon:", Location = new Point(440, 440), AutoSize = true };
            _txtTelefon = new TextBox { Location = new Point(520, 437), Width = 150 };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(440, 470), AutoSize = true };
            _txtEmail = new TextBox { Location = new Point(520, 467), Width = 200 };

            Label lblYetkili = new Label { Text = "Yetkili Kişi:", Location = new Point(440, 500), AutoSize = true };
            _txtYetkili = new TextBox { Location = new Point(520, 497), Width = 200 };

            _chkAktif = new CheckBox { Text = "Aktif", Location = new Point(520, 530), Checked = true };

            // Buttons
            _btnYeni = new Button { Text = "Yeni Kayıt", Location = new Point(20, 580), Width = 100 };
            _btnYeni.Click += BtnYeni_Click;

            _btnDuzelt = new Button { Text = "Düzelt", Location = new Point(130, 580), Width = 100 };
            _btnDuzelt.Click += BtnDuzelt_Click;

            _btnSil = new Button { Text = "Sil", Location = new Point(240, 580), Width = 100 };
            _btnSil.Click += BtnSil_Click;

            // Add controls
            this.Controls.AddRange(new Control[] {
                _grid, lblFirma, _txtFirmaAdi, lblVergi, _txtVergiNo, lblAdres, _txtAdres,
                lblTel, _txtTelefon, lblEmail, _txtEmail, lblYetkili, _txtYetkili, _chkAktif,
                _btnYeni, _btnDuzelt, _btnSil
            });
        }

        private void ListeyiYukle()
        {
            using (var context = new SatinAlmaDbContext())
            {
                var list = context.TedarikciFirmalar
                    .Select(t => new
                    {
                        t.Id,
                        t.FirmaAdi,
                        t.VergiNo,
                        t.Telefon,
                        t.Email,
                        t.YetkiliKisi,
                        Durum = t.Aktif ? "Aktif" : "Pasif"
                    }).ToList();

                _grid.DataSource = list;
            }
        }

        private void Grid_SelectionChanged(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count > 0)
            {
                _seciliId = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
                using (var context = new SatinAlmaDbContext())
                {
                    var firma = context.TedarikciFirmalar.Find(_seciliId);
                    if (firma != null)
                    {
                        _txtFirmaAdi.Text = firma.FirmaAdi;
                        _txtVergiNo.Text = firma.VergiNo;
                        _txtAdres.Text = firma.Adres;
                        _txtTelefon.Text = firma.Telefon;
                        _txtEmail.Text = firma.Email;
                        _txtYetkili.Text = firma.YetkiliKisi;
                        _chkAktif.Checked = firma.Aktif;
                    }
                }
            }
        }

        private void BtnYeni_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtFirmaAdi.Text))
            {
                MessageBox.Show("Firma adı boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new SatinAlmaDbContext())
            {
                var yeni = new TedarikçiFirma
                {
                    FirmaAdi = _txtFirmaAdi.Text,
                    VergiNo = _txtVergiNo.Text,
                    Adres = _txtAdres.Text,
                    Telefon = _txtTelefon.Text,
                    Email = _txtEmail.Text,
                    YetkiliKisi = _txtYetkili.Text,
                    Aktif = _chkAktif.Checked
                };

                context.TedarikciFirmalar.Add(yeni);
                context.SaveChanges();
                MessageBox.Show("Tedarikçi kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListeyiYukle();
                Temizle();
            }
        }

        private void BtnDuzelt_Click(object? sender, EventArgs e)
        {
            if (_seciliId == 0)
            {
                MessageBox.Show("Lütfen düzeltmek için bir firma seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new SatinAlmaDbContext())
            {
                var firma = context.TedarikciFirmalar.Find(_seciliId);
                if (firma != null)
                {
                    firma.FirmaAdi = _txtFirmaAdi.Text;
                    firma.VergiNo = _txtVergiNo.Text;
                    firma.Adres = _txtAdres.Text;
                    firma.Telefon = _txtTelefon.Text;
                    firma.Email = _txtEmail.Text;
                    firma.YetkiliKisi = _txtYetkili.Text;
                    firma.Aktif = _chkAktif.Checked;

                    context.SaveChanges();
                    MessageBox.Show("Tedarikçi güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ListeyiYukle();
                    Temizle();
                }
            }
        }

        private void BtnSil_Click(object? sender, EventArgs e)
        {
            if (_seciliId == 0)
            {
                MessageBox.Show("Lütfen silmek için bir firma seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Bu tedarikçiyi silmek istediğinizden emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                using (var context = new SatinAlmaDbContext())
                {
                    var firma = context.TedarikciFirmalar.Find(_seciliId);
                    if (firma != null)
                    {
                        context.TedarikciFirmalar.Remove(firma);
                        context.SaveChanges();
                        MessageBox.Show("Tedarikçi silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ListeyiYukle();
                        Temizle();
                    }
                }
            }
        }

        private void Temizle()
        {
            _seciliId = 0;
            _txtFirmaAdi.Clear();
            _txtVergiNo.Clear();
            _txtAdres.Clear();
            _txtTelefon.Clear();
            _txtEmail.Clear();
            _txtYetkili.Clear();
            _chkAktif.Checked = true;
        }
    }
}
