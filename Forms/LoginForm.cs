using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using satinalma.Data;
using satinalma.Models.Entities;
using satinalma.Models.Enums;

namespace satinalma.Forms
{
    public class LoginForm : Form
    {
        // Diğer formlara bilgi aktarmak için özellikler
        public bool GirisBasarili { get; private set; } = false;
        public Kullanici? GirisYapanKullanici { get; private set; }

        // Görsel Elemanlar
        private TextBox _txtKadi = new TextBox();
        private TextBox _txtSifre = new TextBox();
        private Button _btnGiris = new Button();

        public LoginForm()
        {
            FormAyarlari();
            BilesenleriKur();
        }

        private void FormAyarlari()
        {
            this.Text = "Kullanıcı Girişi";
            this.Size = new Size(350, 280);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void BilesenleriKur()
        {
            // Bilgi Etiketi
            Label lblBilgi = new Label
            {
                Text = "Varsayılan: admin / 123",
                Location = new Point(20, 10),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // Kullanıcı Adı Alanı
            Label lblUser = new Label { Text = "Kullanıcı Adı:", Location = new Point(30, 60), AutoSize = true };
            _txtKadi.Location = new Point(120, 57);
            _txtKadi.Width = 150;

            // Şifre Alanı
            Label lblPass = new Label { Text = "Şifre:", Location = new Point(30, 100), AutoSize = true };
            _txtSifre.Location = new Point(120, 97);
            _txtSifre.Width = 150;
            _txtSifre.PasswordChar = '*';

            // Giriş Butonu
            _btnGiris.Text = "GİRİŞ YAP";
            _btnGiris.Location = new Point(120, 140);
            _btnGiris.Width = 150;
            _btnGiris.Height = 40;
            _btnGiris.BackColor = Color.SteelBlue;
            _btnGiris.ForeColor = Color.White;
            _btnGiris.Click += _btnGiris_Click;

            // Enter tuşuna basınca giriş yapsın
            this.AcceptButton = _btnGiris;

            // Kontrolleri forma ekle
            this.Controls.Add(lblBilgi);
            this.Controls.Add(lblUser);
            this.Controls.Add(_txtKadi);
            this.Controls.Add(lblPass);
            this.Controls.Add(_txtSifre);
            this.Controls.Add(_btnGiris);
        }

        private void _btnGiris_Click(object? sender, EventArgs e)
        {
            using (var context = new SatinAlmaDbContext())
            {
                var user = context.Kullanicilar
                    .FirstOrDefault(k => k.KullaniciAdi == _txtKadi.Text && k.Sifre == _txtSifre.Text);

                if (user != null)
                {
                    // Giriş Başarılı İşaretle
                    GirisBasarili = true;
                    GirisYapanKullanici = user;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Hatalı kullanıcı adı veya şifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}