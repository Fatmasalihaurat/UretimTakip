using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;
using System;
using System.Linq;

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

        // AJAX ile ürünleri ve depolardaki toplam stoklarını listeleyen motor
        [HttpGet]
        public IActionResult UrunleriListele(string? aramaParametresi = null, bool arsivdekiler = false)
        {
            try
            {
                // Silinmiş (arşivlenmiş) veya aktif ürünler
                var query = _context.Urunler.Where(x => x.IsDeleted == arsivdekiler);

                // Eğer arama kutusuna bir şey yazıldıysa, hem ürün adına hem de ürün koduna göre filtreleme yapıyoruz
                if (!string.IsNullOrEmpty(aramaParametresi))
                {
                    aramaParametresi = aramaParametresi.ToLower();
                    query = query.Where(x => x.UrunAdi.ToLower().Contains(aramaParametresi) || 
                                             x.SistemUrunKodu.ToLower().Contains(aramaParametresi));
                }

                var urunler = query.ToList();

                // Ürünlerin depolardaki aktif stoklarını gruplayarak alalım
                var aktifStoklar = _context.Stoklar
                    .Where(s => !s.IsDeleted)
                    .GroupBy(s => s.UrunId)
                    .Select(g => new { UrunId = g.Key, ToplamStok = g.Sum(x => x.Miktar) })
                    .ToDictionary(x => x.UrunId, x => x.ToplamStok);

                var sonuc = urunler.Select(u =>
                {
                    int toplamStok = aktifStoklar.TryGetValue(u.UrunId, out var stok) ? stok : 0;
                    return new
                    {
                        id = u.UrunId,
                        ad = u.UrunAdi,
                        urunKodu = u.SistemUrunKodu,
                        fiyat = u.Fiyat,
                        toplamStok = toplamStok,
                        silinebilirMi = (toplamStok == 0),
                        isDeleted = u.IsDeleted
                    };
                }).ToList();

                return Json(ResultDto<object>.Success(sonuc, "Ürünler başarıyla getirildi."));
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
                yeniUrun.CreatedDate = DateTime.UtcNow;
                yeniUrun.IsDeleted = false;

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
                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == id);

                if (urun == null)
                {
                    return Json(ResultDto.Failure("Ürün bulunamadı!"));
                }

                return Json(ResultDto<object>.Success(new
                {
                    id = urun.UrunId,
                    ad = urun.UrunAdi,
                    urunKodu = urun.SistemUrunKodu,
                    fiyat = urun.Fiyat
                }, "Ürün bilgileri getirildi."));
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

                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == guncelUrun.Id);

                if (urun == null)
                {
                    return Json(ResultDto.Failure("Güncellenecek ürün bulunamadı!"));
                }

                urun.UrunAdi = guncelUrun.Ad;
                urun.SistemUrunKodu = guncelUrun.UrunKodu;
                urun.Fiyat = guncelUrun.Fiyat;
                urun.GuncellenmeTarihi = DateTime.UtcNow;

                _context.SaveChanges();

                return Json(ResultDto.Success("Ürün başarıyla güncellendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürün güncellenirken hata oluştu: " + ex.Message));
            }
        }

        // Yalnızca Stok Sıfırsa Silme ve Veritabanında Arşivleme (Soft-Delete)
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

                // Bu ürüne ait depolardaki aktif stokları kontrol et
                var aktifStoklar = _context.Stoklar.Where(s => s.UrunId == id && !s.IsDeleted).ToList();
                int toplamStok = aktifStoklar.Sum(s => s.Miktar);

                // Stok sıfır değilse silme işlemi kesinlikle engellenir!
                if (toplamStok > 0)
                {
                    return Json(ResultDto.Failure($"Bu ürüne ait depolarda toplam {toplamStok} adet stok bulunmaktadır. Stoğu olan ürünler silinemez! Lütfen önce depolardaki stok miktarını sıfırlayın veya arşivleyin."));
                }

                // Stok sıfır ise: Ürünü fiziksel olarak silmiyoruz, veritabanında arşiv olarak saklıyoruz (IsDeleted = true)
                urun.IsDeleted = true;
                urun.GuncellenmeTarihi = DateTime.UtcNow;

                // Varsa ürüne ait 0 miktarlı stok kayıtlarını da pasife çekiyoruz
                foreach (var s in aktifStoklar)
                {
                    s.IsDeleted = true;
                    s.SonGuncellenmeTarihi = DateTime.UtcNow;
                }

                _context.SaveChanges();

                return Json(ResultDto.Success("Ürün başarıyla silindi ve arşivlendi! Veritabanında kayıtlı kalmaya devam edecektir."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürün silinirken hata oluştu: " + ex.Message));
            }
        }

        // Arşivlenen Ürünü Geri Yükleme
        [HttpPost]
        public IActionResult UrunGeriYukle(Guid id)
        {
            try
            {
                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == id);

                if (urun == null)
                {
                    return Json(ResultDto.Failure("Ürün bulunamadı!"));
                }

                urun.IsDeleted = false;
                urun.GuncellenmeTarihi = DateTime.UtcNow;

                _context.SaveChanges();

                return Json(ResultDto.Success("Ürün başarıyla arşivden çıkarıldı ve tekrar aktif hale getirildi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürün geri yüklenirken hata oluştu: " + ex.Message));
            }
        }
    }
}
