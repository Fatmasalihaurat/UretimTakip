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
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        public List<SiparisDetayDto> Detaylar { get; set; } = new List<SiparisDetayDto>();
    }
}
