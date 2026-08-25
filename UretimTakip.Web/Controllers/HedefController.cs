using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;
using System;
using System.Linq;

namespace UretimTakip.Web.Controllers
{
    public class HedefController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HedefController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult HedefleriListele()
        {
            try
            {
                var hedefler = _context.Hedefler
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToList();
                return Json(ResultDto<object>.Success(hedefler, "Hedefler başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Hedefler listelenirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult HedefEkle([FromBody] Hedef yeniHedef)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniHedef.Baslik))
                {
                    return Json(ResultDto.Failure("Hedef başlığı boş bırakılamaz!"));
                }
                if (yeniHedef.HedeflenenTutar <= 0)
                {
                    return Json(ResultDto.Failure("Hedeflenen tutar 0'dan büyük olmalıdır!"));
                }
                if (yeniHedef.MevcutTutar < 0)
                {
                    return Json(ResultDto.Failure("Mevcut tutar negatif olamaz!"));
                }

                yeniHedef.HedefId = Guid.NewGuid();
                yeniHedef.OlusturulmaTarihi = DateTime.Now;
                
                if (yeniHedef.MevcutTutar >= yeniHedef.HedeflenenTutar)
                {
                    yeniHedef.TamamlandiMi = true;
                }

                _context.Hedefler.Add(yeniHedef);
                _context.SaveChanges();

                return Json(ResultDto.Success("Yeni hedef başarıyla eklendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Hedef eklenirken hata oluştu: " + ex.Message));
            }
        }

        [HttpGet]
        public IActionResult HedefGetir(Guid id)
        {
            try
            {
                var hedef = _context.Hedefler.FirstOrDefault(x => x.HedefId == id && !x.IsDeleted);

                if (hedef == null)
                {
                    return Json(ResultDto.Failure("Hedef bulunamadı!"));
                }

                return Json(ResultDto<object>.Success(hedef, "Hedef bilgileri getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Hedef getirilirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult HedefGuncelle([FromBody] Hedef guncelHedef)
        {
            try
            {
                if (string.IsNullOrEmpty(guncelHedef.Baslik))
                {
                    return Json(ResultDto.Failure("Hedef başlığı boş bırakılamaz!"));
                }
                if (guncelHedef.HedeflenenTutar <= 0)
                {
                    return Json(ResultDto.Failure("Hedeflenen tutar 0'dan büyük olmalıdır!"));
                }
                if (guncelHedef.MevcutTutar < 0)
                {
                    return Json(ResultDto.Failure("Mevcut tutar negatif olamaz!"));
                }

                var hedef = _context.Hedefler.FirstOrDefault(x => x.HedefId == guncelHedef.HedefId && !x.IsDeleted);

                if (hedef == null)
                {
                    return Json(ResultDto.Failure("Güncellenecek hedef bulunamadı!"));
                }

                hedef.Baslik = guncelHedef.Baslik;
                hedef.HedeflenenTutar = guncelHedef.HedeflenenTutar;
                hedef.MevcutTutar = guncelHedef.MevcutTutar;
                hedef.GuncellenmeTarihi = DateTime.Now;

                if (hedef.MevcutTutar >= hedef.HedeflenenTutar)
                {
                    hedef.TamamlandiMi = true;
                }
                else
                {
                    hedef.TamamlandiMi = false;
                }

                _context.SaveChanges();

                return Json(ResultDto.Success("Hedef başarıyla güncellendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Hedef güncellenirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult HedefSil(Guid id)
        {
            try
            {
                var hedef = _context.Hedefler.FirstOrDefault(x => x.HedefId == id);

                if (hedef == null)
                {
                    return Json(ResultDto.Failure("Hedef bulunamadı!"));
                }

                hedef.IsDeleted = true;
                _context.SaveChanges();

                return Json(ResultDto.Success("Hedef başarıyla silindi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Hedef silinirken hata oluştu: " + ex.Message));
            }
        }
    }
}
