using System;
using System.Collections.Generic;

namespace UretimTakip.core.DTOs
{
    // Siparişin genel bilgilerini ve detay satırlarını bir arada tutan ana DTO
    public class SiparisOlusturDto
    {
        public Guid CariId { get; set; }
        public Guid DepoId { get; set; }
        public string SiparisNumarasi { get; set; } = string.Empty;
        public string SiparisTuru { get; set; } = "Satış"; // "Satış" veya "Alış"
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        public DateTime? VadeTarihi { get; set; }
        public List<SiparisDetayDto> Detaylar { get; set; } = new List<SiparisDetayDto>();
    }

    public class SiparisGuncelleDto
    {
        public Guid SiparisId { get; set; }
        public Guid CariId { get; set; }
        public Guid? DepoId { get; set; }
        public string SiparisTuru { get; set; } = "Satış";
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        public DateTime? VadeTarihi { get; set; }
        public string? Durum { get; set; }
    }
}
