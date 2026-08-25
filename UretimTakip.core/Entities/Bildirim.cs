using System;
using System.ComponentModel.DataAnnotations;

namespace UretimTakip.core.Entities
{
    public class Bildirim
    {
        [Key]
        public Guid BildirimId { get; set; } = Guid.NewGuid();

        [Required]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        public string Mesaj { get; set; } = string.Empty;

        public bool OkunduMu { get; set; }

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? OkunmaTarihi { get; set; }
        public bool IsDeleted { get; set; }
    }
}
