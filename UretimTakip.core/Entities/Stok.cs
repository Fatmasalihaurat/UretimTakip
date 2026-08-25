using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UretimTakip.core.Entities
{
    public class Stok
    {
        [Key]
        public Guid StokId { get; set; } = Guid.NewGuid();

        public Guid DepoId { get; set; }
        public Guid UrunId { get; set; }

        public string StokKodu { get; set; } = string.Empty;
        public int Miktar { get; set; }

        public DateTime SonGuncellenmeTarihi { get; set; } = DateTime.UtcNow;
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public Guid Id { get => StokId; set => StokId = value; }

        [NotMapped]
        public int Adet { get => Miktar; set => Miktar = value; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public DateTime UpdatedDate { get => SonGuncellenmeTarihi; set => SonGuncellenmeTarihi = value; }
    }
}