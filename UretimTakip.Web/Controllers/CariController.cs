using Microsoft.AspNetCore.Mvc;
using UretimTakip.Business.services;
using UretimTakip.core.Models;
using System;

namespace UretimTakip.Web.Controllers
{
    public class CariController : Controller
    {
        private readonly ICariService _cariService;

        public CariController(ICariService cariService)
        {
            _cariService = cariService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Carileri arama ve tür filtresine göre listeleyen AJAX metodu
        [HttpGet]
        public IActionResult CarileriListele(string? aramaKelimesi = null, string? cariTuru = null)
        {
            var result = _cariService.CarileriListele(aramaKelimesi, cariTuru);
            return Json(result);
        }

        // Tek bir cari kaydını düzenleme için getiren AJAX metodu
        [HttpGet]
        public IActionResult CariGetir(Guid id)
        {
            var result = _cariService.CariGetir(id);
            return Json(result);
        }

        // Yeni Cari Ekleme
        [HttpPost]
        public IActionResult CariEkle([FromBody] Cari yeniCari)
        {
            var result = _cariService.CariEkle(yeniCari);
            return Json(result);
        }

        // Cari Güncelleme
        [HttpPost]
        public IActionResult CariGuncelle([FromBody] Cari guncelCari)
        {
            var result = _cariService.CariGuncelle(guncelCari);
            return Json(result);
        }

        // Cari Silme
        [HttpPost]
        public IActionResult CariSil(Guid id)
        {
            var result = _cariService.CariSil(id);
            return Json(result);
        }
    }
}
