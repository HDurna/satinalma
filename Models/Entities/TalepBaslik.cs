using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using satinalma.Models.Enums;

namespace satinalma.Models.Entities
{
    [Table("TalepBaslik")]
    public class TalepBaslik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TalepNo { get; set; } = string.Empty;

        [Required]
        public DateTime TalepTarihi { get; set; }

        public DateTime? SevkTarihi { get; set; }

        [Required]
        [MaxLength(200)]
        public string TalepBirimi { get; set; } = string.Empty;

        [Required]
        public TalepTipi Tip { get; set; }

        public int EkapYapildi { get; set; } = 0;

        [Required]
        public TalepDurumu Durum { get; set; }

        [MaxLength(100)]
        public string TalepEden { get; set; } = string.Empty;

        public int SilmeDurumu { get; set; } = 0;

        [MaxLength(500)]
        public string SilmeAciklamasi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Aciklama { get; set; } = string.Empty;

        public int? SiparisId { get; set; }

        // Navigation property
        public virtual ICollection<TalepDetay> TalepDetaylari { get; set; } = new List<TalepDetay>();
    }
}
