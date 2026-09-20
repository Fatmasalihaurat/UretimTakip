using System;
using System.ComponentModel.DataAnnotations;

namespace UretimTakip.core.Entities
{
    public class Siparis
    {
        [Key]
        public Guid SiparisId { get; set; } = Guid.NewGuid();
        public string SiparisNumarasi { get; set; } = string.Empty;
        public Guid CariId { get; set; } = Guid.NewGuid();
        public Guid? DepoId { get; set; }
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        public DateTime? VadeTarihi { get; set; }
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        public string SiparisTuru { get; set; } = "Satış"; // "Satış" veya "Alış"
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = "Bekliyor";
        public bool IsDeleted { get; set; } = false;
    }
}
