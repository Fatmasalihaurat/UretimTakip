using System;

namespace UretimTakip.core.Entities
{
    public class Stok
    {
        public Guid StokId { get; set; } = Guid.NewGuid();

        public Guid DepoId { get; set; }
        public Guid UrunId { get; set; }

        public string StokKodu { get; set; } = string.Empty;
        public int Miktar { get; set; }

        public DateTime SonGuncellenmeTarihi { get; set; } = DateTime.UtcNow;
        public DateTime OlusturulmaTarihi { get; set; }
        public int Adet { get; set; }
        public Guid Id { get; set; }
    }
}