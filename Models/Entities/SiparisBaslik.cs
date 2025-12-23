using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using satinalma.Models.Enums;

namespace satinalma.Models.Entities
{
    [Table("SiparisBaslik")]
    public class SiparisBaslik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SiparisNo { get; set; } = string.Empty;

        [Required]
        public DateTime SiparisTarihi { get; set; }

        [Required]
        public int TedarikciFirmaId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamTutar { get; set; }

        [Required]
        public SiparisDurumu Durum { get; set; }

        [MaxLength(50)]
        public string OlusturanKullanici { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Aciklama { get; set; } = string.Empty;

        [MaxLength(50)]
        public string EkNo { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("TedarikciFirmaId")]
        public virtual TedarikçiFirma? TedarikçiFirma { get; set; }

        public virtual ICollection<SiparisDetay> SiparisDetaylari { get; set; } = new List<SiparisDetay>();
    }
}
