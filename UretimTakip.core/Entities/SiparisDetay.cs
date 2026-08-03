using System;
using System.ComponentModel.DataAnnotations;

namespace UretimTakip.core.Entities
{
    public class SiparisDetay
    {
        [Key]
        public Guid SiparisDetayId { get; set; } = Guid.NewGuid();
        public Guid SiparisId { get; set; } 
        public Guid UrunId { get; set; }
        public int Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
