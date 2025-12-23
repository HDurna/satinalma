using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("Bildirimler")]
    public class Bildirim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Mesaj { get; set; } = string.Empty;

        [Required]
        public DateTime Tarih { get; set; }

        public int Okundu { get; set; } = 0;
    }
}
