using Microsoft.AspNetCore.Mvc;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UretimTakip.Web.Controllers
{
    public class SiparisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiparisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Siparişlerin listeleneceği ana görünüm (View)
        public IActionResult Index()
        {
            return View();
        }

        // 2. Siparişleri AJAX ile listelemek için GET metodu (Join kullanarak Cari adıyla birlikte)
        [HttpGet]
        public IActionResult SiparisleriListele()
        {
            try
            {
                var siparisler = (from s in _context.Siparisler
                                  join c in _context.Cariler on s.CariId equals c.Id
                                  where !s.IsDeleted
                                  select new
                                  {
                                      SiparisId = s.SiparisId,
                                      SiparisNumarasi = s.SiparisNumarasi,
                                      CariId = s.CariId,
                                      CariAdi = c.Ad,
                                      SiparisTarihi = s.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                                      ToplamTutar = s.ToplamTutar,
                                      Durum = s.Durum
                                  }).ToList();

                return Json(ResultDto<object>.Success(siparisler, "Siparişler başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Siparişler listelenirken hata oluştu: " + ex.Message));
            }
        }

        // 3. Siparişi Master-Detail transaction ile kaydeden POST metodu
        [HttpPost]
        public IActionResult SiparisOlustur([FromBody] SiparisOlusturDto dto)
        {
            if (dto == null)
            {
                return Json(ResultDto.Failure("Sipariş verisi boş olamaz."));
            }

            if (dto.Detaylar == null || !dto.Detaylar.Any())
            {
                return Json(ResultDto.Failure("Sipariş oluşturabilmek için en az bir ürün eklemelisiniz."));
            }

            // Transaction başlatıyoruz. Bu sayede sipariş veya detayların herhangi birinde
            // hata çıkarsa veritabanına yarım yamalak kayıt atılmasını engellemiş oluruz.
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Bugün girilen siparişlerin sayısını alıp sıralı numara üretelim
                var bugun = DateTime.Today;
                var tarihFormat = bugun.ToString("yyyyMMdd");
                var bugunkuSiparisSayisi = _context.Siparisler.Count(s => s.SiparisTarihi.Date == bugun);
                var otomatikSiparisNo = $"SIP-{tarihFormat}-{(bugunkuSiparisSayisi + 1).ToString("D4")}";

                // Sipariş Başlığı (Master) Oluşturma
                var siparis = new Siparis
                {
                    SiparisId = Guid.NewGuid(),
                    CariId = dto.CariId,
                    SiparisNumarasi = otomatikSiparisNo, // Sistem tarafından üretilen sipariş numarası
                    SiparisTarihi = dto.SiparisTarihi,
                    // Detay listesindeki (Miktar * BirimFiyat) değerlerini toplayarak ToplamTutarı hesaplıyoruz
                    ToplamTutar = dto.Detaylar.Sum(x => x.Miktar * x.BirimFiyat),
                    Durum = "Bekliyor",
                    IsDeleted = false
                };

                // Sipariş Başlığını veritabanı bağlamına ekliyoruz
                _context.Siparisler.Add(siparis);

                // Sipariş Detay Satırlarını (Detail) Döngüyle Ekleme
                foreach (var detayDto in dto.Detaylar)
                {
                    // 1. Seçilen depo ve ürüne ait stok kaydını veritabanından sorgula
                    var stok = _context.Stoklar.FirstOrDefault(s => s.DepoId == dto.DepoId && s.UrunId == detayDto.UrunId);

                    // 2. Stok kaydı hiç yoksa hata fırlat (Ürün adını göstermek için önce ürünü çekiyoruz)
                    if (stok == null)
                    {
                        var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == detayDto.UrunId);
                        string urunAdi = urun?.UrunAdi ?? "Bilinmeyen Ürün";
                        throw new Exception($"'{urunAdi}' ürünü için seçilen depoda stok kaydı bulunamadı!");
                    }

                    // 3. Stok miktarı sipariş edilmek istenen miktardan azsa hata fırlat
                    if (stok.Miktar < detayDto.Miktar)
                    {
                        var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == detayDto.UrunId);
                        string urunAdi = urun?.UrunAdi ?? "Bilinmeyen Ürün";
                        throw new Exception($"Yetersiz stok! '{urunAdi}' ürünü için depoda sadece {stok.Miktar} adet mevcut (Sipariş: {detayDto.Miktar} adet).");
                    }

                    // 4. Stok yeterliyse, depodaki stoktan sipariş miktarını düş ve güncelleme tarihini yaz
                    stok.Miktar -= detayDto.Miktar;
                    stok.SonGuncellenmeTarihi = DateTime.UtcNow;

                    // 5. Sipariş detay kaydını oluştur
                    var detay = new SiparisDetay
                    {
                        SiparisDetayId = Guid.NewGuid(),
                        SiparisId = siparis.SiparisId,
                        UrunId = detayDto.UrunId,
                        Miktar = detayDto.Miktar,
                        BirimFiyat = detayDto.BirimFiyat,
                        IsDeleted = false
                    };

                    _context.SiparislerDetaylar.Add(detay);
                }

                // Değişiklikleri fiziksel olarak veritabanına yazıyoruz
                _context.SaveChanges();

                // Her şey başarılıysa yapılan tüm veritabanı işlemlerini onaylıyoruz (Commit)
                transaction.Commit();

                return Json(ResultDto.Success("Sipariş başarıyla oluşturuldu."));
            }
            catch (Exception ex)
            {
                // Herhangi bir adımda hata alınırsa, yapılan tüm işlemleri geri alıyoruz (Rollback)
                transaction.Rollback();
                return Json(ResultDto.Failure("Sipariş oluşturulurken bir hata meydana geldi: " + ex.Message));
            }
        }
        [HttpPost]
        public IActionResult SiparisSil(Guid id)
        {
            try
            {
                var siparis = _context.Siparisler.FirstOrDefault(s => s.SiparisId == id);

                if (siparis == null)
                {
                    return Json(ResultDto.Failure("Sipariş bulunamadı!"));
                }

                // Siparişin kendisini soft-delete yapıyoruz
                siparis.IsDeleted = true;

                // Siparişe bağlı tüm kalemleri (detayları) de soft-delete yapıyoruz
                var detaylar = _context.SiparislerDetaylar.Where(d => d.SiparisId == id).ToList();
                foreach (var detay in detaylar)
                {
                    detay.IsDeleted = true;
                }

                _context.SaveChanges();

                return Json(ResultDto.Success("Sipariş başarıyla iptal edildi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Sipariş iptal edilirken hata oluştu: " + ex.Message));
            }
        }

        // 4. Tek bir siparişin detaylarını getiren GET metodu (Modal ve PDF için)
        [HttpGet]
        public IActionResult SiparisDetayGetir(Guid id)
        {
            try
            {
                // Sipariş bilgilerini ve cari adını alalım
                var siparis = (from s in _context.Siparisler
                               join c in _context.Cariler on s.CariId equals c.Id
                               where s.SiparisId == id && !s.IsDeleted
                               select new
                               {
                                   SiparisId = s.SiparisId,
                                   SiparisNumarasi = s.SiparisNumarasi,
                                   CariId = s.CariId,
                                   CariAdi = c.Ad,
                                   CariKodu = c.CariKodu,
                                   CariTuru = c.CariTuru,
                                   SiparisTarihi = s.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                                   ToplamTutar = s.ToplamTutar,
                                   Durum = s.Durum
                               }).FirstOrDefault();

                if (siparis == null)
                {
                    return Json(ResultDto.Failure("Sipariş bulunamadı!"));
                }

                // Sipariş kalemlerini ve ürün isimlerini alalım
                var detaylar = (from sd in _context.SiparislerDetaylar
                                join u in _context.Urunler on sd.UrunId equals u.UrunId
                                where sd.SiparisId == id && !sd.IsDeleted
                                select new
                                {
                                    UrunKodu = u.SistemUrunKodu,
                                    UrunAdi = u.UrunAdi,
                                    Miktar = sd.Miktar,
                                    BirimFiyat = sd.BirimFiyat,
                                    SatirToplam = sd.Miktar * sd.BirimFiyat
                                }).ToList();

                var sonuc = new
                {
                    Siparis = siparis,
                    Detaylar = detaylar
                };

                return Json(ResultDto<object>.Success(sonuc, "Sipariş detayları başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Sipariş detayları yüklenirken hata oluştu: " + ex.Message));
            }
        }

        // 5. Fatura Yazdırma (PDF) Görünümü (Yeni sekmede açılır ve otomatik yazdırılır)
        [HttpGet]
        public IActionResult FaturaPdf(Guid id)
        {
            try
            {
                // Sipariş ve Müşteri (Cari) bilgilerini alalım
                var fatura = (from s in _context.Siparisler
                               join c in _context.Cariler on s.CariId equals c.Id
                               where s.SiparisId == id && !s.IsDeleted
                               select new FaturaViewModel
                               {
                                   SiparisNumarasi = s.SiparisNumarasi,
                                   CariAdi = c.Ad,
                                   CariKodu = c.CariKodu,
                                   CariTuru = c.CariTuru,
                                   SiparisTarihi = s.SiparisTarihi,
                                   ToplamTutar = s.ToplamTutar,
                                   Durum = s.Durum
                               }).FirstOrDefault();

                if (fatura == null)
                {
                    return Content("Hata: Sipariş bulunamadı!");
                }

                // Sipariş kalemlerini alalım
                fatura.Detaylar = (from sd in _context.SiparislerDetaylar
                                   join u in _context.Urunler on sd.UrunId equals u.UrunId
                                   where sd.SiparisId == id && !sd.IsDeleted
                                   select new FaturaDetayViewModel
                                   {
                                       UrunKodu = u.SistemUrunKodu,
                                       UrunAdi = u.UrunAdi,
                                       Miktar = sd.Miktar,
                                       BirimFiyat = sd.BirimFiyat,
                                       SatirToplam = sd.Miktar * sd.BirimFiyat
                                   }).ToList();

                return View(fatura);
            }
            catch (Exception ex)
            {
                return Content("Fatura yüklenirken hata oluştu: " + ex.Message);
            }
        }
    }

    public class FaturaViewModel
    {
        public string SiparisNumarasi { get; set; } = string.Empty;
        public string CariAdi { get; set; } = string.Empty;
        public string CariKodu { get; set; } = string.Empty;
        public string CariTuru { get; set; } = string.Empty;
        public DateTime SiparisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = string.Empty;
        public List<FaturaDetayViewModel> Detaylar { get; set; } = new();
    }

    public class FaturaDetayViewModel
    {
        public string UrunKodu { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public int Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal SatirToplam { get; set; }
    }
}
