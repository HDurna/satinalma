using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using satinalma.Data;

namespace satinalma.Forms
{
    public class SiparisDetayForm : Form
    {
        private int _siparisId;
        private Label _lblBilgi = null!;
        private DataGridView _gridDetay = null!;

        public SiparisDetayForm(int siparisId)
        {
            _siparisId = siparisId;
            FormGorunum.Uygula(this);
            this.Text = "Sipariş Detayı";
            BilesenleriOlustur();
            DetaylariYukle();
        }

        private void BilesenleriOlustur()
        {
            _lblBilgi = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(900, 100),
                Font = new System.Drawing.Font("Segoe UI", 10)
            };

            _gridDetay = new DataGridView
            {
                Location = new Point(20, 130),
                Size = new Size(900, 400),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Button btnKapat = new Button
            {
                Text = "Kapat",
                Location = new Point(820, 550),
                Width = 100,
                Height = 35
            };
            btnKapat.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { _lblBilgi, _gridDetay, btnKapat });
        }

        private void DetaylariYukle()
        {
            using (var context = new SatinAlmaDbContext())
            {
                var siparis = context.SiparisBasliklar
                    .Where(s => s.Id == _siparisId)
                    .Select(s => new
                    {
                        s.SiparisNo,
                        s.SiparisTarihi,
                        TedarikçiFirma = s.TedarikçiFirma != null ? s.TedarikçiFirma.FirmaAdi : "",
                        s.ToplamTutar,
                        Durum = s.Durum.ToString(),
                        s.OlusturanKullanici,
                        s.Aciklama,
                        s.EkNo
                    })
                    .FirstOrDefault();

                if (siparis != null)
                {
                    _lblBilgi.Text = $"Sipariş No: {siparis.SiparisNo}\n" +
                                    $"Tarih: {siparis.SiparisTarihi:dd.MM.yyyy}\n" +
                                    $"Tedarikçi: {siparis.TedarikçiFirma}\n" +
                                    $"Toplam Tutar: {siparis.ToplamTutar:N2} TL\n" +
                                    $"Durum: {siparis.Durum}\n" +
                                    $"EK No: {siparis.EkNo}";
                }

                var detaylar = context.SiparisDetaylar
                    .Where(d => d.SiparisBaslikId == _siparisId)
                    .Select(d => new
                    {
                        d.UrunAdi,
                        d.Miktar,
                        d.Birim,
                        d.BirimFiyat,
                        d.Tutar,
                        d.Aciklama
                    })
                    .ToList();

                _gridDetay.DataSource = detaylar;
            }
        }
    }
}
