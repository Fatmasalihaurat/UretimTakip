using System;

namespace UretimTakip.core.DTOs
{
    public class SiparisDetayDto
    {
        public Guid UrunId { get; set; }
        public int Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
    }
}