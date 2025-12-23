using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using satinalma.Models.Enums;

namespace satinalma.Models.Entities
{
    [Table("Kullanicilar")]
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string AdSoyad { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Sifre { get; set; } = string.Empty;

        [Required]
        public Rol Yetki { get; set; }
    }
}
