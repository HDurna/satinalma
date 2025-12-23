using System.Drawing;
using System.Windows.Forms;

namespace satinalma
{
    public static class FormGorunum
    {
        public static void Uygula(Form form)
        {
            // Başlangıç konumu
            form.StartPosition = FormStartPosition.CenterScreen;

            // Tam ekran başlasın
            form.WindowState = FormWindowState.Maximized;
            

            // Çok küçük olmasın
            form.MinimumSize = new Size(1100, 700);

            // Ortak font
            form.Font = new Font("Segoe UI", 10);
        }
    }
}
