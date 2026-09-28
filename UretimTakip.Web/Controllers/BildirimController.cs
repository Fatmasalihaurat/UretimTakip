using Microsoft.AspNetCore.Mvc;
using UretimTakip.Business.services;
using System;

namespace UretimTakip.Web.Controllers
{
    public class BildirimController : Controller
    {
        private readonly IBildirimService _bildirimService;

        public BildirimController(IBildirimService bildirimService)
        {
            _bildirimService = bildirimService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BildirimleriListele()
        {
            var result = _bildirimService.BildirimleriListele();
            return Json(result);
        }

        [HttpGet]
        public IActionResult BildirimGetir(Guid id)
        {
            var result = _bildirimService.BildirimGetir(id);
            return Json(result);
        }

        [HttpPost]
        public IActionResult OkunduYap(Guid id)
        {
            var result = _bildirimService.OkunduYap(id);
            return Json(result);
        }

        [HttpPost]
        public IActionResult TumunuOkunduYap()
        {
            var result = _bildirimService.TumunuOkunduYap();
            return Json(result);
        }

        [HttpPost]
        public IActionResult BildirimSil(Guid id)
        {
            var result = _bildirimService.BildirimSil(id);
            return Json(result);
        }

        [HttpGet]
        public IActionResult OkunmamisSayisi()
        {
            var result = _bildirimService.OkunmamisSayisi();
            return Json(result);
        }

        [HttpPost]
        public IActionResult ButceUyarisiOlustur()
        {
            var result = _bildirimService.ButceUyarisiOlustur();
            return Json(result);
        }
    }
}
