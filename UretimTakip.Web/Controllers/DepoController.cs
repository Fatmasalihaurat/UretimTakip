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
        public IActionResult DepolariListele()
        {
            try
            {
                var depolar = _context.Depolar.Where(x => !x.IsArchived).ToList();
                return Json(ResultDto<List<Depo>>.Success(depolar, "Depolar başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Depolar listelenirken hata oluştu: " + ex.Message));
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
