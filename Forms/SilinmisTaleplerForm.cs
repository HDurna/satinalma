using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using satinalma.Data;

namespace satinalma.Forms
{
    public class SilinmisTaleplerForm : Form
    {

        private DataGridView _grid;

        public SilinmisTaleplerForm()
        {
            FormGorunum.Uygula(this);

            this.Text = "Silinmiş Talepler Geçmişi";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.Add(_grid);

            Listele();
        }

        private void Listele()
        {
            _grid.DataSource = Veritabani.VeriGetir(
                "SELECT TalepNo, TalepEden, IslemYapan, IslemTarihi, Aciklama " +
                "FROM SilmeLog WHERE IslemTipi='Onay' ORDER BY Id DESC");
        }
    }
}
