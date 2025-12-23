using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("OlcuBirimleri")]
    public class OlcuBirimi
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string BirimAdi { get; set; } = string.Empty;
    }
}
