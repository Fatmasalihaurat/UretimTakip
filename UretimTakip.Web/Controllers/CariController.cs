using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Models; 
using UretimTakip.DataAccess.Context;
using System;
using System.Linq;

namespace UretimTakip.Web.Controllers
{
    public class CariController : Controller
    {
        private readonly ApplicationDbContext? _context;

        public CariController(ApplicationDbContext? context)
        {
            _context = context;
        }
         public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CarileriListele()
        {
            try
            {
                var cariler = _context.Cariler.Where(x => !x.IsDeleted).ToList();
                return Json(ResultDto<List<Cari>>.Success(cariler, "Cariler başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Cariler listelenirken hata oluştu: " + ex.Message));
            }
        }
        [HttpPost]
        public IActionResult CariEkle([FromBody] Cari yeniCari)
        {
            try
            {
                if(string.IsNullOrEmpty(yeniCari.Ad) || string.IsNullOrEmpty(yeniCari.CariKodu))
                {
                    return Json(ResultDto.Failure("Cari adı veya kodu boş bırakılamaz!"));
                }
                yeniCari.Id = Guid.NewGuid();

                _context.Cariler.Add(yeniCari);
                _context.SaveChanges();

                return Json(ResultDto.Success("Yeni cari başarıyla eklendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Cari oluşturulurken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult CariSil(Guid id)
        {
            try
            {
                var cari = _context.Cariler.FirstOrDefault(c => c.Id == id);

                if (cari == null)
                {
                    return Json(ResultDto.Failure("Cari bulunamadı!"));
                }

                cari.IsDeleted = true;
                _context.SaveChanges();

                return Json(ResultDto.Success("Cari başarıyla silindi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Cari silinirken hata oluştu: " + ex.Message));
            }
        }
    }

}
