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
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        public  decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = "Bekliyor";
        public bool IsDeleted { get; set; } = false;
    }
}
