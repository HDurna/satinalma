using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("TalepDetay")]
    public class TalepDetay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TalepBaslikId { get; set; }

        public int UrunId { get; set; }

        [Required]
        [MaxLength(500)]
        public string UrunAdi { get; set; } = string.Empty;

        [Required]
        public double Miktar { get; set; }

        [Required]
        [MaxLength(50)]
        public string Birim { get; set; } = string.Empty;

        public double SiparisMiktari { get; set; } = 0;

        public double KalanMiktar { get; set; } = 0;

        // Navigation property
        [ForeignKey("TalepBaslikId")]
        public virtual TalepBaslik? TalepBaslik { get; set; }
    }
}
