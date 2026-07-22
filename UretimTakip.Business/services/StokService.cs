using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UretimTakip.core.DTOs;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Business.services
{
    public class StokService
    {
        private readonly ApplicationDbContext _context;

        public StokService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ResultDto StokDus(Guid urunId, Guid depoId, int miktar)
        {
            try
            {
                var mevcutStok = _context.Stoklar.FirstOrDefault(x => x.UrunId == urunId && x.DepoId == depoId && !x.IsDeleted);

                if (mevcutStok == null || mevcutStok.Miktar < miktar)
                {
                    return ResultDto.Failure("İşlem Reddi: Yeterli stok yok! ");
                }

                mevcutStok.Miktar -= miktar;
                mevcutStok.UpdatedDate = DateTime.Now;

                _context.SaveChanges();
                return ResultDto.Success("Stok düşümü yapıldı.");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("HATA! :" + ex.Message);
            }
        }


    }
}
