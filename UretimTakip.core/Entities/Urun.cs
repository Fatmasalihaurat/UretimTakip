using System;

namespace UretimTakip.core.Entities
{
    public class Urun
    {
        // Temel Tanımlayıcılar
        public Guid UrunId { get; set; } = Guid.NewGuid(); // Benzersiz anahtar (Primary Key) 
        public string SistemUrunKodu { get; set; } = string.Empty; // Otomatik üretilecek eşsiz kod 
        public string HariciKodu { get; set; } = string.Empty; // Kullanıcının manuel girdiyi kod 

        // Ürün Detayları
        public string UrunAdi { get; set; } = string.Empty; 
        public string Marka { get; set; } = string.Empty; 
        public string Model { get; set; } = string.Empty; 
        public string Kategori { get; set; } = string.Empty; 
        public string OlcuBirimi { get; set; } = string.Empty; // Litre, Adet, Kilogram vb.
        public decimal Fiyat { get; set; } 

        // Fiziksel Özellikler 
        public double Uzunluk { get; set; }
        
        public double Genislik { get; set; }
        public double Yukseklik { get; set; }
        public double Agirlik { get; set; }
        public double Desi { get; set; }
        
        // Teknik Özellikler 
        public string Akim { get; set; } = string.Empty; 
        public string Voltaj { get; set; } = string.Empty; 
        public string Guc { get; set; } = string.Empty; 

        // Sistem Takip Alanları
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? GuncellenmeTarihi { get; set; }
    }
}