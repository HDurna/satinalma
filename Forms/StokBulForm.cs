using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using satinalma.Data;

namespace satinalma.Forms
{
    public class StokBulForm : Form
    {
        public int SecilenUrunId { get; private set; } = 0;
        public string SecilenUrunTamAdi { get; private set; } = string.Empty;

        // Arama Kutuları
        private TextBox _txtKategori;
        private TextBox _txtUrunAdi;
        private TextBox _txtOzellik;
        private DataGridView _grid;

        public StokBulForm()
        {
            this.Text = "Stok Kartı Arama";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            ArayuzKur();
            AramaYap();
        }

        private void ArayuzKur()
        {
            GroupBox grpFiltre = new GroupBox { Text = "Filtreler", Location = new Point(10, 10), Size = new Size(910, 80) };

            Label lbl1 = new Label { Text = "Kategori:", Location = new Point(20, 30), AutoSize = true };
            _txtKategori = new TextBox { Location = new Point(80, 27), Width = 150 };
            _txtKategori.TextChanged += (s, e) => AramaYap();

            Label lbl2 = new Label { Text = "Ürün Adı:", Location = new Point(250, 30), AutoSize = true };
            _txtUrunAdi = new TextBox { Location = new Point(310, 27), Width = 150 };
            _txtUrunAdi.TextChanged += (s, e) => AramaYap();

            // Artık buraya yazılan 3 özellik içinde de aranacak
            Label lbl3 = new Label { Text = "Özellik Ara:", Location = new Point(480, 30), AutoSize = true };
            _txtOzellik = new TextBox { Location = new Point(560, 27), Width = 150, PlaceholderText = "Renk, Boyut, Tip..." };
            _txtOzellik.TextChanged += (s, e) => AramaYap();

            grpFiltre.Controls.AddRange(new Control[] { lbl1, _txtKategori, lbl2, _txtUrunAdi, lbl3, _txtOzellik });
            this.Controls.Add(grpFiltre);

            _grid = new DataGridView
            {
                Location = new Point(10, 100),
                Size = new Size(910, 450),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false
            };

            _grid.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                {
                    SecilenUrunId = Convert.ToInt32(_grid.Rows[e.RowIndex].Cells["Id"].Value);
                    SecilenUrunTamAdi = _grid.Rows[e.RowIndex].Cells["TamAd"].Value.ToString();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };
            this.Controls.Add(_grid);
        }

        private void AramaYap()
        {
            // 3. Özelliği de sorguya ekledik
            string sql = "SELECT Id, KategoriAdi, UrunAdi, Ozellik1, Ozellik2, Ozellik3, TamAd FROM Urunler WHERE 1=1";

            if (!string.IsNullOrEmpty(_txtKategori.Text)) sql += $" AND KategoriAdi LIKE '%{_txtKategori.Text}%'";
            if (!string.IsNullOrEmpty(_txtUrunAdi.Text)) sql += $" AND UrunAdi LIKE '%{_txtUrunAdi.Text}%'";

            // Herhangi bir özellik sütununda aranan kelime varsa getir
            if (!string.IsNullOrEmpty(_txtOzellik.Text))
                sql += $" AND (Ozellik1 LIKE '%{_txtOzellik.Text}%' OR Ozellik2 LIKE '%{_txtOzellik.Text}%' OR Ozellik3 LIKE '%{_txtOzellik.Text}%')";

            sql += " ORDER BY UrunAdi";

            _grid.DataSource = Veritabani.VeriGetir(sql);
            if (_grid.Columns["Id"] != null) _grid.Columns["Id"].Visible = false;
        }
    }
}