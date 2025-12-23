using System;
using System.Drawing;
using System.Windows.Forms;
using satinalma.Models.Entities;
using satinalma.Models.Enums;

namespace satinalma.Forms
{
    public class MainForm : Form
    {
        private Kullanici _aktifKullanici = null!;
        public bool OturumuKapatildi { get; private set; } = false;

        // Boş ctor (tasarımcı / hata önleyici)
        public MainForm() { }

        public MainForm(Kullanici kullanici)
        {
            _aktifKullanici = kullanici;

            FormGorunum.Uygula(this);
            this.Text = $"Ana Menü - {_aktifKullanici.KullaniciAdi}";

            ArayuzOlustur();
        }

        private void ArayuzOlustur()
        {
            // Ana layout: 3 satır
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));             // header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));             // menü butonları
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));         // dashboard / boş alan
            this.Controls.Add(mainLayout);

            // ---------- 1) HEADER ----------
            var headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 50,
                Padding = new Padding(15, 10, 15, 5)
            };

            Label lblHosgeldin = new Label
            {
                Text = $"Hoşgeldin, {_aktifKullanici.AdSoyad} ({_aktifKullanici.Yetki})",
                AutoSize = true,
                Dock = DockStyle.Left,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40)
            };

            Button btnCikis = new Button
            {
                Text = "Oturumu Kapat",
                Width = 130,
                Height = 32,
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCikis.FlatAppearance.BorderSize = 0;
            btnCikis.Click += (s, e) =>
            {
                OturumuKapatildi = true;
                this.Close();
            };

            // Sağ tarafa hizalamak için küçük bir panel
            var rightPanel = new Panel { Dock = DockStyle.Right, Width = 140 };
            rightPanel.Controls.Add(btnCikis);
            btnCikis.Location = new Point(rightPanel.Width - btnCikis.Width, 0);

            headerPanel.Controls.Add(lblHosgeldin);
            headerPanel.Controls.Add(rightPanel);

            mainLayout.Controls.Add(headerPanel, 0, 0);

            // ---------- 2) MENÜ BUTONLARI ----------
            var menuPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                Padding = new Padding(20, 10, 20, 10)
            };

            menuPanel.Controls.Add(MenuButonu("Talep Girişi", Color.Teal));
            menuPanel.Controls.Add(MenuButonu("Sipariş Girişi", Color.MediumSeaGreen));
            menuPanel.Controls.Add(MenuButonu("Tanımlamalar", Color.MediumPurple));
            menuPanel.Controls.Add(MenuButonu("Kullanıcı Yönetimi", Color.DarkOrange));
            menuPanel.Controls.Add(MenuButonu("Raporlar", Color.Gold));
            menuPanel.Controls.Add(MenuButonu("Silme Talepleri", Color.Crimson));
            menuPanel.Controls.Add(MenuButonu("Bildirimler", Color.SteelBlue));

            mainLayout.Controls.Add(menuPanel, 0, 1);

            // ---------- 3) ORTA ALAN (Dashboard Placeholder) ----------
            var dashboardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(20)
            };

            Label lblDashboard = new Label
            {
                Text = "Dashboard / Rapor Alanı\n\n" +
                       "İleride grafikler, özet sayılar, son talepler vs. burada gösterilebilir.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DimGray
            };

            dashboardPanel.Controls.Add(lblDashboard);
            mainLayout.Controls.Add(dashboardPanel, 0, 2);

            // ---------- ALT BİLGİ ÇUBUĞU ----------
            StatusStrip statusStrip = new StatusStrip();
            ToolStripStatusLabel lblBosluk = new ToolStripStatusLabel { Spring = true };
            ToolStripStatusLabel lblBilgi = new ToolStripStatusLabel
            {
                Text = $"Aktif Kullanıcı: {_aktifKullanici.KullaniciAdi} | Yetki: {_aktifKullanici.Yetki}",
                ForeColor = Color.DarkBlue,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            statusStrip.Items.Add(lblBosluk);
            statusStrip.Items.Add(lblBilgi);
            statusStrip.Dock = DockStyle.Bottom;
            this.Controls.Add(statusStrip);
        }

        private Button MenuButonu(string yazi, Color renk)
        {
            Button btn = new Button
            {
                Text = yazi,
                Width = 150,
                Height = 80,
                BackColor = renk,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(10)
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.Click += (s, e) =>
            {
                if (yazi == "Talep Girişi")
                {
                    using (var frm = new TalepListesiForm(_aktifKullanici))
                        frm.ShowDialog(this);
                }
                else if (yazi == "Sipariş Girişi")
                {
                    using (var frm = new SiparisListesiForm(_aktifKullanici))
                        frm.ShowDialog(this);
                }
                else if (yazi == "Tanımlamalar")
                {
                    using (var frm = new TanimlamalarForm())
                        frm.ShowDialog(this);
                }
                else if (yazi == "Silme Talepleri")
                {
                    using (var frm = new SilmeOnayForm())
                        frm.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show(
                        $"{yazi} ekranı henüz hazırlanmadı. Daha sonra eklenecek.",
                        "Bilgi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            };

            return btn;
        }
    }
}
