using Microsoft.AspNetCore.Mvc;
using UretimTakip.Business.services;
using UretimTakip.core.Entities;
using System;

namespace UretimTakip.Web.Controllers
{
    public class DepoController : Controller
    {
        private readonly IDepoService _depoService;

        public DepoController(IDepoService depoService)
        {
            _depoService = depoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DepolariListele(string? aramaParametresi = null)
        {
            var result = _depoService.DepolariListele(aramaParametresi);
            return Json(result);
        }

        [HttpGet]
        public IActionResult DepoGetir(Guid id)
        {
            var result = _depoService.DepoGetir(id);
            return Json(result);
        }

        [HttpPost]
        public IActionResult DepoGuncelle([FromBody] Depo guncelDepo)
        {
            var result = _depoService.DepoGuncelle(guncelDepo);
            return Json(result);
        }

        [HttpPost]
        public IActionResult DepoEkle([FromBody] Depo yeniDepo)
        {
            var result = _depoService.DepoEkle(yeniDepo);
            return Json(result);
        }

        [HttpPost]
        public IActionResult DepoSil(Guid id)
        {
            var result = _depoService.DepoSil(id);
            return Json(result);
        }
    }
}
