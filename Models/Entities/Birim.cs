using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("Birimler")]
    public class Birim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string BirimAdi { get; set; } = string.Empty;

        [MaxLength(100)]
        public string HarcamaYetkilisi { get; set; } = string.Empty;
    }
}
