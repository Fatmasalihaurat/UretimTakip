using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UretimTakip.core.Models
{
    public class Cari
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Ad { get; set; } = string.Empty;
        public string CariKodu { get; set; } =string.Empty;
        public string CariTuru { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}
