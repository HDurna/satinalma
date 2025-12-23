using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("SilmeLog")]
    public class SilmeLog
    {
        [Key]
        public int Id { get; set; }

        public int TalepBaslikId { get; set; }

        [MaxLength(50)]
        public string TalepNo { get; set; } = string.Empty;

        [Required]
        public DateTime IslemTarihi { get; set; }

        [Required]
        [MaxLength(50)]
        public string IslemTipi { get; set; } = string.Empty;

        [MaxLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Aciklama { get; set; } = string.Empty;
    }
}
