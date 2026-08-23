using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Web.Controllers
{
    public class DepoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DepolariListele(string aramaParametresi)
        {
            try
            {
                var sorgu = _context.Depolar.Where(x => !x.IsArchived).AsQueryable();

                if (!string.IsNullOrEmpty(aramaParametresi))
                {
                    aramaParametresi = aramaParametresi.ToLower();
                    sorgu = sorgu.Where(x => x.DepoAdi.ToLower().Contains(aramaParametresi) || 
                                             x.DepoKodu.ToLower().Contains(aramaParametresi));
                }

                var depolar = sorgu.ToList();
                return Json(ResultDto<List<Depo>>.Success(depolar, "Depolar başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Depolar listelenirken hata oluştu: " + ex.Message));
            }
        }

        [HttpGet]
        public IActionResult DepoGetir(Guid id)
        {
            try
            {
                var depo = _context.Depolar.FirstOrDefault(d => d.DepoId == id && !d.IsArchived);
                if (depo == null)
                {
                    return Json(ResultDto.Failure("Depo bulunamadı!"));
                }
                return Json(ResultDto<Depo>.Success(depo, "Depo bilgileri getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Depo getirilirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult DepoGuncelle([FromBody] Depo guncelDepo)
        {
            try
            {
                if (string.IsNullOrEmpty(guncelDepo.Ad) || string.IsNullOrEmpty(guncelDepo.DepoKodu))
                {
                    return Json(ResultDto.Failure("Depo adı veya kodu boş bırakılamaz!"));
                }

                var depo = _context.Depolar.FirstOrDefault(d => d.DepoId == guncelDepo.Id);
                if (depo == null)
                {
                    return Json(ResultDto.Failure("Güncellenecek depo bulunamadı!"));
                }

                depo.DepoAdi = guncelDepo.Ad;
                depo.DepoKodu = guncelDepo.DepoKodu;
                depo.SorumluKisi = guncelDepo.SorumluKisi;
                depo.Konum = guncelDepo.Konum;
                depo.IrtibatBilgisi = guncelDepo.IrtibatBilgisi;
                depo.Adres = guncelDepo.Adres;

                _context.SaveChanges();

                return Json(ResultDto.Success("Depo başarıyla güncellendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Depo güncellenirken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult DepoEkle([FromBody] Depo yeniDepo)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniDepo.Ad) || string.IsNullOrEmpty(yeniDepo.DepoKodu))
                {
                    return Json(ResultDto.Failure("Depo adı veya kodu boş bırakılamaz!"));
                }

                yeniDepo.Id = Guid.NewGuid();
                yeniDepo.OlusturulmaTarihi = DateTime.Now;

                _context.Depolar.Add(yeniDepo);
                _context.SaveChanges();

                return Json(ResultDto.Success("Yeni depo başarıyla eklendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Depo oluşturulurken hata oluştu: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult DepoSil(Guid id)
        {
            try
            {
                var depo = _context.Depolar.FirstOrDefault(d => d.DepoId == id);

                if (depo == null)
                {
                    return Json(ResultDto.Failure("Depo bulunamadı!"));
                }

                depo.IsArchived = true;
                _context.SaveChanges();

                return Json(ResultDto.Success("Depo başarıyla silindi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Depo silinirken hata oluştu: " + ex.Message));
            }
        }
    }
}
