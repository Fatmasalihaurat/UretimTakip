using Microsoft.AspNetCore.Mvc;
using UretimTakip.Business.services;
using UretimTakip.core.Entities;
using System;

namespace UretimTakip.Web.Controllers
{
    public class HedefController : Controller
    {
        private readonly IHedefService _hedefService;

        public HedefController(IHedefService hedefService)
        {
            _hedefService = hedefService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult HedefleriListele()
        {
            var result = _hedefService.HedefleriListele();
            return Json(result);
        }

        [HttpPost]
        public IActionResult HedefEkle([FromBody] Hedef yeniHedef)
        {
            var result = _hedefService.HedefEkle(yeniHedef);
            return Json(result);
        }

        [HttpGet]
        public IActionResult HedefGetir(Guid id)
        {
            var result = _hedefService.HedefGetir(id);
            return Json(result);
        }

        [HttpPost]
        public IActionResult HedefGuncelle([FromBody] Hedef guncelHedef)
        {
            var result = _hedefService.HedefGuncelle(guncelHedef);
            return Json(result);
        }

        [HttpPost]
        public IActionResult HedefSil(Guid id)
        {
            var result = _hedefService.HedefSil(id);
            return Json(result);
        }
    }
}
