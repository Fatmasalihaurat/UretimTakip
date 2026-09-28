using Microsoft.AspNetCore.Mvc;
using UretimTakip.Business.services;
using UretimTakip.core.Entities;
using System;

namespace UretimTakip.Web.Controllers
{
    public class UrunController : Controller
    {
        private readonly IUrunService _urunService;

        public UrunController(IUrunService urunService)
        {
            _urunService = urunService;
        }

        // Sayfayı açan ana görünüm
        public IActionResult Index()
        {
            return View();
        }

        // AJAX ile ürünleri ve depolardaki toplam stoklarını listeleyen endpoint
        [HttpGet]
        public IActionResult UrunleriListele(string? aramaParametresi = null, bool arsivdekiler = false)
        {
            var result = _urunService.UrunleriListele(aramaParametresi, arsivdekiler);
            return Json(result);
        }

        // AJAX ile yeni ürün ekleyen endpoint
        [HttpPost]
        public IActionResult UrunEkle([FromBody] Urun yeniUrun)
        {
            var result = _urunService.UrunEkle(yeniUrun);
            return Json(result);
        }

        // Düzenleme için ürün bilgilerini getiren endpoint
        [HttpGet]
        public IActionResult UrunGetir(Guid id)
        {
            var result = _urunService.UrunGetir(id);
            return Json(result);
        }

        // Ürün güncelleyen endpoint
        [HttpPost]
        public IActionResult UrunGuncelle([FromBody] Urun guncelUrun)
        {
            var result = _urunService.UrunGuncelle(guncelUrun);
            return Json(result);
        }

        // Yalnızca Stok Sıfırsa Silme ve Veritabanında Arşivleme (Soft-Delete)
        [HttpPost]
        public IActionResult UrunSil(Guid id)
        {
            var result = _urunService.UrunSil(id);
            return Json(result);
        }

        // Arşivlenen Ürünü Geri Yükleme
        [HttpPost]
        public IActionResult UrunGeriYukle(Guid id)
        {
            var result = _urunService.UrunGeriYukle(id);
            return Json(result);
        }
    }
}
