using Microsoft.AspNetCore.Mvc;
using UretimTakip.Business.services;
using UretimTakip.core.DTOs;
using System;

namespace UretimTakip.Web.Controllers
{
    public class SiparisController : Controller
    {
        private readonly ISiparisService _siparisService;

        public SiparisController(ISiparisService siparisService)
        {
            _siparisService = siparisService;
        }

        // 1. Siparişlerin listeleneceği ana görünüm (View)
        public IActionResult Index()
        {
            return View();
        }

        // 2. Siparişleri Arama, Sipariş Türü (Alış/Satış) ve Durum Filtreleriyle AJAX ile listeleyen GET metodu
        [HttpGet]
        public IActionResult SiparisleriListele(string? aramaKelimesi = null, string? siparisTuru = null, string? durum = null)
        {
            var result = _siparisService.SiparisleriListele(aramaKelimesi, siparisTuru, durum);
            return Json(result);
        }

        // 3. Siparişi Master-Detail transaction ile kaydeden POST metodu
        [HttpPost]
        public IActionResult SiparisOlustur([FromBody] SiparisOlusturDto dto)
        {
            var result = _siparisService.SiparisOlustur(dto);
            return Json(result);
        }

        // 4. Sipariş Durumunu Güncelleyen (Tamamla / İptal Et) POST Metodu
        [HttpPost]
        public IActionResult SiparisDurumGuncelle(Guid id, string yeniDurum)
        {
            var result = _siparisService.SiparisDurumGuncelle(id, yeniDurum);
            return Json(result);
        }

        // 5. Siparişi İptal Eden (Geriye uyumluluk için SiparisSil)
        [HttpPost]
        public IActionResult SiparisSil(Guid id)
        {
            var result = _siparisService.SiparisSil(id);
            return Json(result);
        }

        // 6. Düzenleme için tek bir siparişin başlık bilgilerini getiren GET metodu
        [HttpGet]
        public IActionResult SiparisGetir(Guid id)
        {
            var result = _siparisService.SiparisGetir(id);
            return Json(result);
        }

        // 7. Sipariş Başlık Bilgilerini Güncelleyen POST Metodu
        [HttpPost]
        public IActionResult SiparisGuncelle([FromBody] SiparisGuncelleDto dto)
        {
            var result = _siparisService.SiparisGuncelle(dto);
            return Json(result);
        }

        // 8. Tek bir siparişin kalem ve özet detaylarını getiren GET metodu (Modal ve PDF için)
        [HttpGet]
        public IActionResult SiparisDetayGetir(Guid id)
        {
            var result = _siparisService.SiparisDetayGetir(id);
            return Json(result);
        }

        // 9. Fatura / İrsaliye Yazdırma (PDF) Görünümü
        [HttpGet]
        public IActionResult FaturaPdf(Guid id)
        {
            var result = _siparisService.FaturaGetir(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return Content("Hata: " + (result.Message ?? "Sipariş bulunamadı!"));
            }

            return View(result.Data);
        }
    }
}
