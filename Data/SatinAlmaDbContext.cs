using Microsoft.EntityFrameworkCore;
using satinalma.Models.Entities;
using satinalma.Models.Enums;

namespace satinalma.Data
{
    public class SatinAlmaDbContext : DbContext
    {
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Birim> Birimler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<OlcuBirimi> OlcuBirimleri { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<TalepBaslik> TalepBasliklar { get; set; }
        public DbSet<TalepDetay> TalepDetaylar { get; set; }
        public DbSet<SilmeLog> SilmeLoglar { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<TedarikçiFirma> TedarikciFirmalar { get; set; }
        public DbSet<SiparisBaslik> SiparisBasliklar { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylar { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\MSSQLLocalDB;Database=SatinAlmaDB;Integrated Security=true;TrustServerCertificate=true;",
                    options => options.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enum to string conversion
            modelBuilder.Entity<Kullanici>()
                .Property(k => k.Yetki)
                .HasConversion<string>();

            modelBuilder.Entity<TalepBaslik>()
                .Property(t => t.Durum)
                .HasConversion<string>();

            modelBuilder.Entity<TalepBaslik>()
                .Property(t => t.Tip)
                .HasConversion<string>();

            modelBuilder.Entity<SiparisBaslik>()
                .Property(s => s.Durum)
                .HasConversion<string>();

            // Relationships
            modelBuilder.Entity<TalepDetay>()
                .HasOne(td => td.TalepBaslik)
                .WithMany(tb => tb.TalepDetaylari)
                .HasForeignKey(td => td.TalepBaslikId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.SiparisBaslik)
                .WithMany(sb => sb.SiparisDetaylari)
                .HasForeignKey(sd => sd.SiparisBaslikId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SiparisBaslik>()
                .HasOne(sb => sb.TedarikçiFirma)
                .WithMany()
                .HasForeignKey(sb => sb.TedarikciFirmaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial admin user
            modelBuilder.Entity<Kullanici>().HasData(
                new Kullanici
                {
                    Id = 1,
                    AdSoyad = "Admin User",
                    KullaniciAdi = "admin",
                    Sifre = "123",
                    Yetki = Rol.Admin
                }
            );

            // Unique indexes
            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.KullaniciAdi)
                .IsUnique();

            modelBuilder.Entity<TalepBaslik>()
                .HasIndex(t => t.TalepNo)
                .IsUnique();

            modelBuilder.Entity<SiparisBaslik>()
                .HasIndex(s => s.SiparisNo)
                .IsUnique();
        }
    }
}
