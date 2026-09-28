using Microsoft.AspNetCore.Mvc;
using UretimTakip.Business.services;
using UretimTakip.core.Entities;
using System;

namespace UretimTakip.Web.Controllers
{
    public class StokController : Controller
    {
        private readonly IStokService _stokService;

        public StokController(IStokService stokService)
        {
            _stokService = stokService;
        }

        // Ana Stok Kontrol Sayfası
        public IActionResult Index(Guid? depoId)
        {
            ViewBag.SeciliDepoId = depoId;
            return View();
        }

        // Stokları Ürün ve Depo Adıyla Birlikte Getiren AJAX Metodu
        [HttpGet]
        public IActionResult StoklariListele(Guid? depoId, string? aramaKelimesi = null, bool arsivdekiler = false)
        {
            var result = _stokService.StoklariListele(depoId, aramaKelimesi, arsivdekiler);
            return Json(result);
        }

        // Aktif Ürünlerin Listesini Dönen AJAX Metodu (Dropdown doldurmak için)
        [HttpGet]
        public IActionResult GetUrunler()
        {
            var result = _stokService.GetUrunler();
            return Json(result);
        }

        // Aktif Depoların Listesini Dönen AJAX Metodu (Dropdown doldurmak için)
        [HttpGet]
        public IActionResult GetDepolar()
        {
            var result = _stokService.GetDepolar();
            return Json(result);
        }

        // Yeni Stok Kaydı Oluşturan AJAX Metodu
        [HttpPost]
        public IActionResult StokEkle([FromBody] Stok yeniStok)
        {
            var result = _stokService.StokEkle(yeniStok);
            return Json(result);
        }

        // Seçilen Stok Miktarını Güncelleyen AJAX Metodu
        [HttpPost]
        public IActionResult StokGuncelle(Guid id, int yeniMiktar)
        {
            var result = _stokService.StokGuncelle(id, yeniMiktar);
            return Json(result);
        }

        // Stoğu Yalnızca Sıfır Olan Kaydı Arşivleme (Soft-Delete) Metodu
        [HttpPost]
        public IActionResult StokSil(Guid id)
        {
            var result = _stokService.StokSil(id);
            return Json(result);
        }

        // Arşivlenen Stok Kaydını Yeniden Aktifleştirme (Restore)
        [HttpPost]
        public IActionResult StokGeriYukle(Guid id)
        {
            var result = _stokService.StokGeriYukle(id);
            return Json(result);
        }
    }
}
