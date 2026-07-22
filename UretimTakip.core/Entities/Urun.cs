using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UretimTakip.core.Entities
{
    public class Urun
    {
        [Key]
        public Guid UrunId { get; set; } = Guid.NewGuid();
        public string SistemUrunKodu { get; set; } = string.Empty;
        public string HariciKodu { get; set; } = string.Empty;

        public string UrunAdi { get; set; } = string.Empty;
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public string OlcuBirimi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }

        public double Uzunluk { get; set; }
        public double Genislik { get; set; }
        public double Yukseklik { get; set; }
        public double Agirlik { get; set; }
        public double Desi { get; set; }

        public string Akim { get; set; } = string.Empty;
        public string Voltaj { get; set; } = string.Empty;
        public string Guc { get; set; } = string.Empty;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? GuncellenmeTarihi { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public Guid Id { get => UrunId; set => UrunId = value; }

        [NotMapped]
        public string? Ad { get => UrunAdi; set => UrunAdi = value ?? string.Empty; }

        [NotMapped]
        public string? UrunKodu { get => SistemUrunKodu; set => SistemUrunKodu = value ?? string.Empty; }

        [NotMapped]
        public DateTime CreatedDate { get => OlusturulmaTarihi; set => OlusturulmaTarihi = value; }
    }
}