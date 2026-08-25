using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;
using System;
using System.Linq;

namespace UretimTakip.Web.Controllers
{
    public class StokController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StokController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ana Stok Kontrol Sayfası
        public IActionResult Index(Guid? depoId)
        {
            ViewBag.SeciliDepoId = depoId;
            return View();
        }

        // Stokları Ürün ve Depo Adıyla Birlikte Getiren AJAX Metodu
        [HttpGet]
        public IActionResult StoklariListele(Guid? depoId, string aramaKelimesi = null)
        {
            try
            {
                // Stok silinmemiş (!s.IsDeleted), ürün silinmemiş (!u.IsDeleted) ve depo arşivlenmemiş (!d.IsArchived) olanları getiriyoruz.
                var sorgu = from s in _context.Stoklar
                            join u in _context.Urunler on s.UrunId equals u.UrunId
                            join d in _context.Depolar on s.DepoId equals d.DepoId
                            where !s.IsDeleted && !u.IsDeleted && !d.IsArchived
                            select new { s, u, d };

                if (depoId.HasValue)
                {
                    sorgu = sorgu.Where(x => x.s.DepoId == depoId.Value);
                }

                if (!string.IsNullOrEmpty(aramaKelimesi))
                {
                    aramaKelimesi = aramaKelimesi.ToLower();
                    sorgu = sorgu.Where(x => x.u.UrunAdi.ToLower().Contains(aramaKelimesi) || 
                                             x.s.StokKodu.ToLower().Contains(aramaKelimesi));
                }

                var stoklar = sorgu.ToList().Select(x => new
                {
                    StokId = x.s.StokId,
                    StokKodu = x.s.StokKodu,
                    UrunId = x.s.UrunId,
                    DepoId = x.s.DepoId,
                    UrunAdi = x.u.UrunAdi,
                    DepoAdi = x.d.DepoAdi,
                    Miktar = x.s.Miktar,
                    SonGuncellenmeTarihi = x.s.SonGuncellenmeTarihi.ToString("dd.MM.yyyy HH:mm")
                }).ToList();

                return Json(ResultDto<object>.Success(stoklar, "Stoklar başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Stoklar listelenirken hata oluştu: " + ex.Message));
            }
        }

        // Aktif Ürünlerin Listesini Dönen AJAX Metodu (Dropdown doldurmak için)
        [HttpGet]
        public IActionResult GetUrunler()
        {
            try
            {
                var urunler = _context.Urunler
                    .Where(x => !x.IsDeleted)
                    .Select(x => new { x.UrunId, x.UrunAdi })
                    .ToList();
                return Json(ResultDto<object>.Success(urunler, "Ürünler başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Ürünler listelenirken hata oluştu: " + ex.Message));
            }
        }

        // Aktif Depoların Listesini Dönen AJAX Metodu (Dropdown doldurmak için)
        [HttpGet]
        public IActionResult GetDepolar()
        {
            try
            {
                var depolar = _context.Depolar
                    .Where(x => !x.IsArchived)
                    .Select(x => new { x.DepoId, x.DepoAdi })
                    .ToList();
                return Json(ResultDto<object>.Success(depolar, "Depolar başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Depolar listelenirken hata oluştu: " + ex.Message));
            }
        }

        // Yeni Stok Kaydı (Bağlantısı) Oluşturan AJAX Metodu
        [HttpPost]
        public IActionResult StokEkle([FromBody] Stok yeniStok)
        {
            try
            {
                if (yeniStok.UrunId == Guid.Empty)
                {
                    return Json(ResultDto.Failure("Lütfen bir ürün seçin!"));
                }
                if (yeniStok.DepoId == Guid.Empty)
                {
                    return Json(ResultDto.Failure("Lütfen bir depo seçin!"));
                }
                if (string.IsNullOrEmpty(yeniStok.StokKodu))
                {
                    return Json(ResultDto.Failure("Stok kodu boş bırakılamaz!"));
                }
                if (yeniStok.Miktar < 0)
                {
                    return Json(ResultDto.Failure("Başlangıç stok miktarı 0 veya daha büyük olmalıdır!"));
                }

                // Aynı ürünün aynı depoda zaten bir stok kaydı var mı kontrol ediyoruz
                var varMi = _context.Stoklar.Any(x => x.UrunId == yeniStok.UrunId && x.DepoId == yeniStok.DepoId);
                if (varMi)
                {
                    return Json(ResultDto.Failure("Bu ürün bu depoda zaten tanımlanmış! Lütfen mevcut stok miktarını güncelleyin."));
                }

                yeniStok.StokId = Guid.NewGuid();
                yeniStok.OlusturulmaTarihi = DateTime.Now;
                yeniStok.SonGuncellenmeTarihi = DateTime.Now;

                _context.Stoklar.Add(yeniStok);
                _context.SaveChanges();

                return Json(ResultDto.Success("Yeni stok kaydı başarıyla oluşturuldu!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Stok kaydı oluşturulurken hata oluştu: " + ex.Message));
            }
        }

        // Seçilen Stok Miktarını Güncelleyen AJAX Metodu
        [HttpPost]
        public IActionResult StokGuncelle(Guid id, int yeniMiktar)
        {
            try
            {
                if (yeniMiktar < 0)
                {
                    return Json(ResultDto.Failure("Stok miktarı 0'dan küçük olamaz!"));
                }

                var stok = _context.Stoklar.FirstOrDefault(x => x.StokId == id);
                if (stok == null)
                {
                    return Json(ResultDto.Failure("Stok kaydı bulunamadı!"));
                }

                if (yeniMiktar == 0)
                {
                    stok.IsDeleted = true;
                    stok.Miktar = 0;
                    stok.SonGuncellenmeTarihi = DateTime.Now;
                    _context.SaveChanges();
                    return Json(ResultDto.Success("Stok miktarı 0 olduğu için stok kaydı arşivlendi!"));
                }

                // Stok miktarını güncelle ve son güncelleme tarihini yenile
                stok.Miktar = yeniMiktar;
                stok.SonGuncellenmeTarihi = DateTime.Now;

                _context.SaveChanges();

                return Json(ResultDto.Success("Stok miktarı başarıyla güncellendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Stok güncellenirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult StokSil(Guid id)
        {
            try
            {
                var stok = _context.Stoklar.FirstOrDefault(x => x.StokId == id);
                if (stok == null)
                {
                    return Json(ResultDto.Failure("Stok kaydı bulunamadı!"));
                }

                stok.IsDeleted = true;
                stok.Miktar = 0;
                stok.SonGuncellenmeTarihi = DateTime.Now;
                _context.SaveChanges();

                return Json(ResultDto.Success("Stok kaydı başarıyla silindi (arşivlendi)!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Stok silinirken hata oluştu: " + ex.Message));
            }
        }
    }
}
