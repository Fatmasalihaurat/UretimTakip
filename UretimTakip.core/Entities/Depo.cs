using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UretimTakip.core.Entities
{
    public class Depo
    {
        [Key]
        public Guid DepoId { get; set; } = Guid.NewGuid();
        public string DepoKodu { get; set; } = string.Empty;
        public string DepoAdi { get; set; } = string.Empty;

        public string Adres { get; set; } = string.Empty;
        public string IrtibatBilgisi { get; set; } = string.Empty;
        public string SorumluKisi { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsArchived { get; set; } = false;
        public string Konum { get; set; } = string.Empty;
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public Guid Id { get => DepoId; set => DepoId = value; }

        [NotMapped]
        public string Ad { get => DepoAdi; set => DepoAdi = value ?? string.Empty; }
    }
}
