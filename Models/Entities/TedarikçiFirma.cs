using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("TedarikciFirmalar")]
    public class TedarikçiFirma
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string FirmaAdi { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string FirmaTipi { get; set; } = "Ticari"; // "Ticari" veya "Şahıs"

        [MaxLength(20)]
        public string VergiNo { get; set; } = string.Empty;

        public DateTime? DogumTarihi { get; set; } // Sadece şahıs şirketleri için

        [MaxLength(500)]
        public string Adres { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefon { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string YetkiliKisi { get; set; } = string.Empty;

        public bool Aktif { get; set; } = true;
    }
}
