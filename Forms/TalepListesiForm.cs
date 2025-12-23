using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using satinalma.Data;
using satinalma.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace satinalma.Forms
{
    public class TalepListesiForm : Form
    {
        private DataGridView _grid;
        private Button _btnYeni, _btnSil;
        private readonly Kullanici _aktifKullanici;

        public TalepListesiForm(Kullanici aktifKullanici)
        {
            _aktifKullanici = aktifKullanici;
            FormGorunum.Uygula(this);
            Text = "Talep Listesi";
            Size = new Size(1000, 600);

            InitializeControls();
            ListeyiYukle();
        }

        private void InitializeControls()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Padding = new Padding(10)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            // Grid
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            // Butonlar
            var pnlButon = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };

            _btnYeni = new Button { Text = "+ Yeni Talep", Width = 120, Height = 35, BackColor = Color.MediumSeaGreen, ForeColor = Color.White };
            _btnYeni.Click += _btnYeni_Click;

            _btnSil = new Button { Text = "Sil", Width = 100, Height = 35, BackColor = Color.Crimson, ForeColor = Color.White };
            _btnSil.Click += _btnSil_Click;

            pnlButon.Controls.AddRange(new Control[] { _btnYeni, _btnSil });

            layout.Controls.Add(_grid, 0, 0);
            layout.Controls.Add(pnlButon, 0, 1);
            Controls.Add(layout);
        }

        private void ListeyiYukle()
        {
            using (var db = new SatinAlmaDbContext())
            {
                // Kullanici tablosuyla birlestirerek verileri cekiyoruz
                var list = db.TalepBasliklar
                             .Include(t => t.Kullanici)
                             .OrderByDescending(t => t.Id)
                             .Select(t => new
                             {
                                 t.Id,
                                 t.TalepNo,
                                 t.TalepTarihi,
                                 Kullanici = t.Kullanici != null ? t.Kullanici.AdSoyad : "Bilinmiyor",
                                 t.Tip,
                                 t.Durum,
                                 t.Aciklama
                             })
                             .ToList();

                _grid.DataSource = list;
            }
        }

        private void _btnYeni_Click(object? sender, EventArgs e)
        {
            // DUZELTME: Constructor'a _aktifKullanici parametresini ekledik
            var form = new TalepEkleForm(_aktifKullanici);
            form.ShowDialog();
            ListeyiYukle();
        }

        private void _btnSil_Click(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Bu talebi silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int id = (int)_grid.SelectedRows[0].Cells["Id"].Value;
                    using (var db = new SatinAlmaDbContext())
                    {
                        var talep = db.TalepBasliklar.Find(id);
                        if (talep != null)
                        {
                            // Direkt veritabanından siliyoruz (Soft delete degil, hard delete)
                            db.TalepBasliklar.Remove(talep);
                            db.SaveChanges();
                        }
                    }
                    ListeyiYukle();
                }
            }
        }
    }
}