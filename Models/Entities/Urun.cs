using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("Urunler")]
    public class Urun
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string KategoriAdi { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string UrunAdi { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Ozellik1 { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Ozellik2 { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Ozellik3 { get; set; } = string.Empty;

        [MaxLength(800)]
        public string TamAd { get; set; } = string.Empty;
    }
}
