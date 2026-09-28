using System;
using System.Linq;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Business.services
{
    public class HedefService : IHedefService
    {
        private readonly ApplicationDbContext _context;

        public HedefService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ResultDto<object> HedefleriListele()
        {
            try
            {
                var hedefler = _context.Hedefler
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToList();
                return ResultDto<object>.Success(hedefler, "Hedefler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Hedefler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<object> HedefGetir(Guid id)
        {
            try
            {
                var hedef = _context.Hedefler.FirstOrDefault(x => x.HedefId == id && !x.IsDeleted);

                if (hedef == null)
                {
                    return ResultDto<object>.Failure("Hedef bulunamadı!");
                }

                return ResultDto<object>.Success(hedef, "Hedef bilgileri getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Hedef getirilirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto HedefEkle(Hedef yeniHedef)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniHedef.Baslik))
                {
                    return ResultDto.Failure("Hedef başlığı boş bırakılamaz!");
                }
                if (yeniHedef.HedeflenenTutar <= 0)
                {
                    return ResultDto.Failure("Hedeflenen tutar 0'dan büyük olmalıdır!");
                }
                if (yeniHedef.MevcutTutar < 0)
                {
                    return ResultDto.Failure("Mevcut tutar negatif olamaz!");
                }

                yeniHedef.HedefId = Guid.NewGuid();
                yeniHedef.OlusturulmaTarihi = DateTime.Now;
                
                if (yeniHedef.MevcutTutar >= yeniHedef.HedeflenenTutar)
                {
                    yeniHedef.TamamlandiMi = true;
                }

                _context.Hedefler.Add(yeniHedef);
                _context.SaveChanges();

                return ResultDto.Success("Yeni hedef başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Hedef eklenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto HedefGuncelle(Hedef guncelHedef)
        {
            try
            {
                if (string.IsNullOrEmpty(guncelHedef.Baslik))
                {
                    return ResultDto.Failure("Hedef başlığı boş bırakılamaz!");
                }
                if (guncelHedef.HedeflenenTutar <= 0)
                {
                    return ResultDto.Failure("Hedeflenen tutar 0'dan büyük olmalıdır!");
                }
                if (guncelHedef.MevcutTutar < 0)
                {
                    return ResultDto.Failure("Mevcut tutar negatif olamaz!");
                }

                var hedef = _context.Hedefler.FirstOrDefault(x => x.HedefId == guncelHedef.HedefId && !x.IsDeleted);

                if (hedef == null)
                {
                    return ResultDto.Failure("Güncellenecek hedef bulunamadı!");
                }

                hedef.Baslik = guncelHedef.Baslik;
                hedef.HedeflenenTutar = guncelHedef.HedeflenenTutar;
                hedef.MevcutTutar = guncelHedef.MevcutTutar;
                hedef.TamamlandiMi = guncelHedef.MevcutTutar >= guncelHedef.HedeflenenTutar;

                _context.SaveChanges();

                return ResultDto.Success("Hedef başarıyla güncellendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Hedef güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto HedefSil(Guid id)
        {
            try
            {
                var hedef = _context.Hedefler.FirstOrDefault(x => x.HedefId == id && !x.IsDeleted);

                if (hedef == null)
                {
                    return ResultDto.Failure("Hedef bulunamadı!");
                }

                hedef.IsDeleted = true;
                _context.SaveChanges();

                return ResultDto.Success("Hedef başarıyla silindi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Hedef silinirken hata oluştu: " + ex.Message);
            }
        }
    }
}
