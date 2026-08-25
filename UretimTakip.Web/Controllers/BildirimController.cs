using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;
using System;
using System.Linq;

namespace UretimTakip.Web.Controllers
{
    public class BildirimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BildirimController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BildirimleriListele()
        {
            try
            {
                var bildirimler = _context.Bildirimler
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToList();
                return Json(ResultDto<object>.Success(bildirimler, "Bildirimler başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Bildirimler listelenirken hata oluştu: " + ex.Message));
            }
        }

        [HttpGet]
        public IActionResult BildirimGetir(Guid id)
        {
            try
            {
                var bildirim = _context.Bildirimler.FirstOrDefault(x => x.BildirimId == id && !x.IsDeleted);
                if (bildirim == null) return Json(ResultDto.Failure("Bildirim bulunamadı!"));
                return Json(ResultDto<object>.Success(bildirim, "Bildirim getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Bildirim getirilirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult OkunduYap(Guid id)
        {
            try
            {
                var bildirim = _context.Bildirimler.FirstOrDefault(x => x.BildirimId == id && !x.IsDeleted);
                if (bildirim != null)
                {
                    bildirim.OkunduMu = true;
                    bildirim.OkunmaTarihi = DateTime.Now;
                    _context.SaveChanges();
                }
                return Json(ResultDto.Success("Bildirim okundu olarak işaretlendi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("İşlem sırasında hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult TumunuOkunduYap()
        {
            try
            {
                var bildirimler = _context.Bildirimler.Where(x => !x.IsDeleted && !x.OkunduMu).ToList();
                foreach (var b in bildirimler)
                {
                    b.OkunduMu = true;
                    b.OkunmaTarihi = DateTime.Now;
                }
                _context.SaveChanges();
                return Json(ResultDto.Success("Tüm bildirimler okundu olarak işaretlendi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("İşlem sırasında hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult BildirimSil(Guid id)
        {
            try
            {
                var bildirim = _context.Bildirimler.FirstOrDefault(x => x.BildirimId == id);
                if (bildirim != null)
                {
                    bildirim.IsDeleted = true;
                    _context.SaveChanges();
                }
                return Json(ResultDto.Success("Bildirim silindi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("İşlem sırasında hata oluştu: " + ex.Message));
            }
        }

        [HttpGet]
        public IActionResult OkunmamisSayisi()
        {
            try
            {
                var count = _context.Bildirimler.Count(x => !x.IsDeleted && !x.OkunduMu);
                return Json(ResultDto<int>.Success(count, "Okunmamış bildirim sayısı getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult ButceUyarisiOlustur()
        {
            try
            {
                var yeniBildirim = new Bildirim
                {
                    BildirimId = Guid.NewGuid(),
                    Baslik = "Bütçe Uyarısı",
                    Mesaj = "Dikkat: Aylık bütçe sınırına yaklaşmaktasınız. Lütfen harcamalarınızı gözden geçirin.",
                    OkunduMu = false,
                    OlusturulmaTarihi = DateTime.Now
                };
                
                _context.Bildirimler.Add(yeniBildirim);
                _context.SaveChanges();
                return Json(ResultDto.Success("Bütçe uyarısı bildirimi oluşturuldu."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Bildirim oluşturulurken hata oluştu: " + ex.Message));
            }
        }
    }
}
