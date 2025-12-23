using satinalma.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    public class Urun
    {
        public int Id { get; set; }

        [Required]
        public string UrunAdi { get; set; }

        public string? Aciklama { get; set; }

        // Ýliþkiler
        public int BirimId { get; set; }
        public Birim Birim { get; set; }

        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; }

        // BU EKSÝKTÝ, EKLÝYORUZ:
        public bool Aktif { get; set; } = true;
    }
}