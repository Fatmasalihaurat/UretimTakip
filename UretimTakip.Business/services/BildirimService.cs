using System;
using System.Linq;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Business.services
{
    public class BildirimService : IBildirimService
    {
        private readonly ApplicationDbContext _context;

        public BildirimService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ResultDto<object> BildirimleriListele()
        {
            try
            {
                var bildirimler = _context.Bildirimler
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToList();
                return ResultDto<object>.Success(bildirimler, "Bildirimler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Bildirimler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<object> BildirimGetir(Guid id)
        {
            try
            {
                var bildirim = _context.Bildirimler.FirstOrDefault(x => x.BildirimId == id && !x.IsDeleted);
                if (bildirim == null) return ResultDto<object>.Failure("Bildirim bulunamadı!");
                return ResultDto<object>.Success(bildirim, "Bildirim getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Bildirim getirilirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto OkunduYap(Guid id)
        {
            try
            {
                var bildirim = _context.Bildirimler.FirstOrDefault(x => x.BildirimId == id && !x.IsDeleted);
                if (bildirim != null)
                {
                    bildirim.OkunduMu = true;
                    bildirim.OkunmaTarihi = DateTime.Now;
                    _context.SaveChanges();
                }
                return ResultDto.Success("Bildirim okundu olarak işaretlendi.");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("İşlem sırasında hata oluştu: " + ex.Message);
            }
        }

        public ResultDto TumunuOkunduYap()
        {
            try
            {
                var bildirimler = _context.Bildirimler.Where(x => !x.IsDeleted && !x.OkunduMu).ToList();
                foreach (var b in bildirimler)
                {
                    b.OkunduMu = true;
                    b.OkunmaTarihi = DateTime.Now;
                }
                _context.SaveChanges();
                return ResultDto.Success("Tüm bildirimler okundu olarak işaretlendi.");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("İşlem sırasında hata oluştu: " + ex.Message);
            }
        }

        public ResultDto BildirimSil(Guid id)
        {
            try
            {
                var bildirim = _context.Bildirimler.FirstOrDefault(x => x.BildirimId == id);
                if (bildirim != null)
                {
                    bildirim.IsDeleted = true;
                    _context.SaveChanges();
                }
                return ResultDto.Success("Bildirim silindi.");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("İşlem sırasında hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<int> OkunmamisSayisi()
        {
            try
            {
                var count = _context.Bildirimler.Count(x => !x.IsDeleted && !x.OkunduMu);
                return ResultDto<int>.Success(count, "Okunmamış bildirim sayısı getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<int>.Failure("Hata oluştu: " + ex.Message);
            }
        }

        public ResultDto ButceUyarisiOlustur()
        {
            try
            {
                var yeniBildirim = new Bildirim
                {
                    BildirimId = Guid.NewGuid(),
                    Baslik = "Bütçe Uyarısı",
                    Mesaj = "Dikkat: Aylık bütçe sınırına yaklaşmaktasınız. Lütfen harcamalarınızı gözden geçirin.",
                    OkunduMu = false,
                    OlusturulmaTarihi = DateTime.Now
                };
                
                _context.Bildirimler.Add(yeniBildirim);
                _context.SaveChanges();
                return ResultDto.Success("Bütçe uyarısı bildirimi oluşturuldu.");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Bildirim oluşturulurken hata oluştu: " + ex.Message);
            }
        }
    }
}
