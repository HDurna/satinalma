using satinalma.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace satinalma.Models.Entities
{
    public class TalepBaslik
    {
        public int Id { get; set; }

        [Required]
        public string TalepNo { get; set; }

        public DateTime TalepTarihi { get; set; } // Adýný senin istediðin gibi TalepTarihi yaptým
        public DateTime SevkTarihi { get; set; }  // GERÝ GELDÝ

        public string Aciklama { get; set; }
        public string TalepBirimi { get; set; }   // GERÝ GELDÝ
        public string TalepEden { get; set; }     // GERÝ GELDÝ

        public TalepDurumu Durum { get; set; }
        public TalepTipi Tip { get; set; }

        public bool EkapYapildi { get; set; }     // GERÝ GELDÝ

        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }

        public ICollection<TalepDetay> TalepDetaylari { get; set; }
    }
}