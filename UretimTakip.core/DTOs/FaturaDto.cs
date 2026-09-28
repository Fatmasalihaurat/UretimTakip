using System;
using System.Collections.Generic;

namespace UretimTakip.core.DTOs
{
    public class FaturaDto
    {
        public string SiparisNumarasi { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public string CariKodu { get; set; } = string.Empty;
        public string CariTuru { get; set; } = string.Empty;
        public string DepoAdi { get; set; } = string.Empty;
        public string SiparisTuru { get; set; } = "Satış";
        public DateTime SiparisTarihi { get; set; }
        public DateTime? VadeTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = string.Empty;
        public List<FaturaDetayDto> Detaylar { get; set; } = new();
    }

    public class FaturaDetayDto
    {
        public string UrunKodu { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public int Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal SatirToplam { get; set; }
    }
}
