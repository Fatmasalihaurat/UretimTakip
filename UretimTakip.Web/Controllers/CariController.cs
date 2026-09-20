using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Models; 
using UretimTakip.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UretimTakip.Web.Controllers
{
    public class CariController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CariController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Carileri arama ve tür filtresine göre listeleyen AJAX metodu
        [HttpGet]
        public IActionResult CarileriListele(string? aramaKelimesi = null, string? cariTuru = null)
        {
            try
            {
                var query = _context.Cariler.Where(x => !x.IsDeleted);

                if (!string.IsNullOrEmpty(cariTuru))
                {
                    query = query.Where(x => x.CariTuru.ToLower() == cariTuru.ToLower());
                }

                if (!string.IsNullOrEmpty(aramaKelimesi))
                {
                    aramaKelimesi = aramaKelimesi.ToLower();
                    query = query.Where(x => x.Ad.ToLower().Contains(aramaKelimesi) || 
                                             x.CariKodu.ToLower().Contains(aramaKelimesi));
                }

                var cariler = query.ToList();
                return Json(ResultDto<List<Cari>>.Success(cariler, "Cariler başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Cariler listelenirken hata oluştu: " + ex.Message));
            }
        }

        // Tek bir cari kaydını düzenleme için getiren AJAX metodu
        [HttpGet]
        public IActionResult CariGetir(Guid id)
        {
            try
            {
                var cari = _context.Cariler.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
                if (cari == null)
                {
                    return Json(ResultDto.Failure("Cari bulunamadı!"));
                }

                return Json(ResultDto<Cari>.Success(cari, "Cari bilgileri başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Cari bilgisi getirilirken hata oluştu: " + ex.Message));
            }
        }

        // Yeni Cari Ekleme
        [HttpPost]
        public IActionResult CariEkle([FromBody] Cari yeniCari)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniCari.Ad) || string.IsNullOrEmpty(yeniCari.CariKodu))
                {
                    return Json(ResultDto.Failure("Cari adı veya kodu boş bırakılamaz!"));
                }

                if (string.IsNullOrEmpty(yeniCari.CariTuru))
                {
                    return Json(ResultDto.Failure("Lütfen cari türünü (Müşteri veya Tedarikçi) seçin!"));
                }

                yeniCari.Id = Guid.NewGuid();
                yeniCari.Ad = yeniCari.Ad.Trim();
                yeniCari.CariKodu = yeniCari.CariKodu.Trim();
                yeniCari.IsDeleted = false;

                _context.Cariler.Add(yeniCari);
                _context.SaveChanges();

                return Json(ResultDto.Success("Yeni cari başarıyla eklendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Cari oluşturulurken hata oluştu: " + ex.Message));
            }
        }

        // Cari Güncelleme
        [HttpPost]
        public IActionResult CariGuncelle([FromBody] Cari guncelCari)
        {
            try
            {
                if (string.IsNullOrEmpty(guncelCari.Ad) || string.IsNullOrEmpty(guncelCari.CariKodu))
                {
                    return Json(ResultDto.Failure("Cari adı veya kodu boş bırakılamaz!"));
                }

                if (string.IsNullOrEmpty(guncelCari.CariTuru))
                {
                    return Json(ResultDto.Failure("Lütfen cari türünü seçin!"));
                }

                var cari = _context.Cariler.FirstOrDefault(c => c.Id == guncelCari.Id && !c.IsDeleted);
                if (cari == null)
                {
                    return Json(ResultDto.Failure("Güncellenecek cari bulunamadı!"));
                }

                cari.Ad = guncelCari.Ad.Trim();
                cari.CariKodu = guncelCari.CariKodu.Trim();
                cari.CariTuru = guncelCari.CariTuru;

                _context.SaveChanges();

                return Json(ResultDto.Success("Cari bilgileri başarıyla güncellendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Cari güncellenirken hata oluştu: " + ex.Message));
            }
        }

        // Cari Silme (Soft-Delete)
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
