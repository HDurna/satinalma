using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("SiparisDetay")]
    public class SiparisDetay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SiparisBaslikId { get; set; }

        public int? TalepDetayId { get; set; }

        [Required]
        [MaxLength(500)]
        public string UrunAdi { get; set; } = string.Empty;

        [Required]
        public double Miktar { get; set; }

        [Required]
        [MaxLength(50)]
        public string Birim { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyat { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tutar { get; set; }

        public DateTime? TeslimTarihi { get; set; }

        [MaxLength(500)]
        public string Aciklama { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey("SiparisBaslikId")]
        public virtual SiparisBaslik? SiparisBaslik { get; set; }
    }
}
