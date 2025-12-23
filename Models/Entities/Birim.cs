using System.ComponentModel.DataAnnotations;

namespace satinalma.Models.Entities
{
    public class Birim
    {
        public int Id { get; set; }

        [Required]
        public string BirimAdi { get; set; } // Adý kesinlikle "BirimAdi" olsun
    }
}