using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Web.Controllers
{
    public class UrunController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UrunController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sayfayı açan ana aksiyon
        public IActionResult Index()
        {
            return View();
        }

        // AJAX ile ürünleri listeleyen motor
        [HttpGet]
        public IActionResult UrunleriListele()
        {
            try
            {
                var urunler = _context.Urunler.Where(x => !x.IsDeleted).ToList();
                return Json(ResultDto<List<Urun>>.Success(urunler, "Ürünler başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürünler listelenirken hata oluştu: " + ex.Message));
            }
        }

        // AJAX ile yeni ürün ekleyen motor
        [HttpPost]
        public IActionResult UrunEkle([FromBody] Urun yeniUrun)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniUrun.Ad) || string.IsNullOrEmpty(yeniUrun.UrunKodu))
                {
                    return Json(ResultDto.Failure("Ürün adı veya kodu boş bırakılamaz!"));
                }

                yeniUrun.Id = Guid.NewGuid();
                yeniUrun.CreatedDate = DateTime.Now;

                _context.Urunler.Add(yeniUrun);
                _context.SaveChanges();

                return Json(ResultDto.Success("Yeni ürün başarıyla eklendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürün eklenirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult UrunSil(Guid id)
        {
            try
            {
                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == id);

                if (urun == null)
                {
                    return Json(ResultDto.Failure("Ürün bulunamadı!"));
                }

                urun.IsDeleted = true;
                _context.SaveChanges();

                return Json(ResultDto.Success("Ürün başarıyla silindi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürün silinirken hata oluştu: " + ex.Message));
            }
        }

    }
}
