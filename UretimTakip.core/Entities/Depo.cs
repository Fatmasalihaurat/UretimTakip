using System;

namespace UretimTakip.core.Entities
{
    public class Depo
    {
        // Dokümanda beklenen alanlar (Model Yapısı)
        public Guid DepoId { get; set; } = Guid.NewGuid(); 
        public string DepoKodu { get; set; } = string.Empty; 
        public string DepoAdi { get; set; } = string.Empty; 

        // Adres ve İrtibat Bilgileri
        public string Adres { get; set; } = string.Empty; 
        public string IrtibatBilgisi { get; set; } = string.Empty; 
        public string SorumluKisi { get; set; } = string.Empty;

        // Durum Alanları (Silme yerine arşivleme kuralı için)
        public bool IsActive { get; set; } = true; 
        public bool IsArchived { get; set; } = false;
        public Guid Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Konum { get; set; } = string.Empty;
        public DateTime OlusturulmaTarihi { get; set; }
    }
}
