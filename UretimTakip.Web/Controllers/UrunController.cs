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
        public IActionResult UrunleriListele(string aramaParametresi = null)
        {
            try
            {
                var query = _context.Urunler.Where(x => !x.IsDeleted);

                // Eğer arama kutusuna bir şey yazıldıysa, hem ürün adına hem de ürün koduna göre filtreleme yapıyoruz
                if (!string.IsNullOrEmpty(aramaParametresi))
                {
                    query = query.Where(x => x.Ad.Contains(aramaParametresi) || x.UrunKodu.Contains(aramaParametresi));
                }

                var urunler = query.ToList();
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

        [HttpGet]
        public IActionResult UrunGetir(Guid id)
        {
            try
            {
                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == id && !u.IsDeleted);

                if (urun == null)
                {
                    return Json(ResultDto.Failure("Ürün bulunamadı!"));
                }

                return Json(ResultDto<Urun>.Success(urun, "Ürün bilgileri getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürün getirilirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult UrunGuncelle([FromBody] Urun guncelUrun)
        {
            try
            {
                if (string.IsNullOrEmpty(guncelUrun.Ad) || string.IsNullOrEmpty(guncelUrun.UrunKodu))
                {
                    return Json(ResultDto.Failure("Ürün adı veya kodu boş bırakılamaz!"));
                }

                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == guncelUrun.Id && !u.IsDeleted);

                if (urun == null)
                {
                    return Json(ResultDto.Failure("Güncellenecek ürün bulunamadı!"));
                }

                urun.UrunAdi = guncelUrun.Ad;
                urun.SistemUrunKodu = guncelUrun.UrunKodu;
                urun.Fiyat = guncelUrun.Fiyat;
                urun.GuncellenmeTarihi = DateTime.Now;

                _context.SaveChanges();

                return Json(ResultDto.Success("Ürün başarıyla güncellendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürün güncellenirken hata oluştu: " + ex.Message));
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
