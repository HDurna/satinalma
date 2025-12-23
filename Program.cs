using System;
using System.Windows.Forms;
using satinalma.Forms;
using satinalma.Data;
using satinalma.Models.Entities;
using satinalma.Models.Enums;

namespace satinalma
{
    static class Program
    {
        public static Kullanici AktifKullanici;

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Entity Framework Core ile veritabanını başlat
            using (var context = new SatinAlmaDbContext())
            {
                //context.Database.EnsureCreated();
            }

            bool programCalisiyor = true;

            while (programCalisiyor)
            {
                LoginForm login = new LoginForm();
                Application.Run(login);

                if (login.GirisBasarili && login.GirisYapanKullanici != null)
                {
                    AktifKullanici = login.GirisYapanKullanici;
                    MainForm main = new MainForm(AktifKullanici);
                    Application.Run(main);
                    programCalisiyor = main.OturumuKapatildi;
                }
                else
                {
                    programCalisiyor = false;
                }
            }
        }
    }
}