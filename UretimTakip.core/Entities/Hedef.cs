using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UretimTakip.core.Entities
{
    public class Hedef
    {
        [Key]
        public Guid HedefId { get; set; } = Guid.NewGuid();

        [Required]
        public string Baslik { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal HedeflenenTutar { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MevcutTutar { get; set; }

        public bool TamamlandiMi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? GuncellenmeTarihi { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public decimal IlerlemeYuzdesi
        {
            get
            {
                if (HedeflenenTutar <= 0) return 0;
                var percentage = (MevcutTutar / HedeflenenTutar) * 100;
                return percentage > 100 ? 100 : Math.Round(percentage, 2);
            }
        }
    }
}
