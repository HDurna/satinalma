using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace satinalma.Models.Entities
{
    [Table("Kategoriler")]
    public class Kategori
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string KategoriAdi { get; set; } = string.Empty;
    }
}
