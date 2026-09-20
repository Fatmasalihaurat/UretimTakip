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

        // 2. Siparişleri Arama, Sipariş Türü (Alış/Satış) ve Durum Filtreleriyle AJAX ile listeleyen GET metodu
        [HttpGet]
        public IActionResult SiparisleriListele(string? aramaKelimesi = null, string? siparisTuru = null, string? durum = null)
        {
            try
            {
                var query = from s in _context.Siparisler
                            join c in _context.Cariler on s.CariId equals c.Id
                            join d in _context.Depolar on s.DepoId equals d.DepoId into depogroup
                            from d in depogroup.DefaultIfEmpty()
                            select new { s, c, d };

                // Sipariş Türü Filtresi (Satış / Alış)
                if (!string.IsNullOrEmpty(siparisTuru))
                {
                    query = query.Where(x => x.s.SiparisTuru.ToLower() == siparisTuru.ToLower());
                }

                // Durum Filtresi (Bekliyor / Tamamlandı / İptal Edildi)
                if (!string.IsNullOrEmpty(durum))
                {
                    if (durum == "İptal Edildi")
                    {
                        query = query.Where(x => x.s.Durum == "İptal Edildi" || x.s.IsDeleted);
                    }
                    else
                    {
                        query = query.Where(x => x.s.Durum == durum && !x.s.IsDeleted);
                    }
                }

                // Arama Kelimesi Filtresi (Sipariş No, Cari Adı, Depo Adı)
                if (!string.IsNullOrEmpty(aramaKelimesi))
                {
                    aramaKelimesi = aramaKelimesi.ToLower();
                    query = query.Where(x => x.s.SiparisNumarasi.ToLower().Contains(aramaKelimesi) ||
                                             x.c.Ad.ToLower().Contains(aramaKelimesi) ||
                                             (x.d != null && x.d.DepoAdi.ToLower().Contains(aramaKelimesi)));
                }

                var siparisler = query
                    .OrderByDescending(x => x.s.SiparisTarihi)
                    .ToList()
                    .Select(x => new
                    {
                        SiparisId = x.s.SiparisId,
                        SiparisNumarasi = x.s.SiparisNumarasi,
                        CariId = x.s.CariId,
                        CariAdi = x.c.Ad,
                        CariKodu = x.c.CariKodu,
                        CariTuru = x.c.CariTuru,
                        DepoId = x.s.DepoId,
                        DepoAdi = x.d != null ? x.d.DepoAdi : "Merkez Depo",
                        SiparisTuru = string.IsNullOrEmpty(x.s.SiparisTuru) ? "Satış" : x.s.SiparisTuru,
                        SiparisTarihi = x.s.SiparisTarihi.ToString("dd.MM.yyyy"),
                        SiparisTarihiRaw = x.s.SiparisTarihi.ToString("yyyy-MM-dd"),
                        VadeTarihi = x.s.VadeTarihi.HasValue ? x.s.VadeTarihi.Value.ToString("dd.MM.yyyy") : "-",
                        VadeTarihiRaw = x.s.VadeTarihi.HasValue ? x.s.VadeTarihi.Value.ToString("yyyy-MM-dd") : "",
                        OlusturulmaTarihi = x.s.OlusturulmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                        ToplamTutar = x.s.ToplamTutar,
                        Durum = x.s.Durum,
                        IsDeleted = x.s.IsDeleted
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

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var siparisTuru = string.IsNullOrEmpty(dto.SiparisTuru) ? "Satış" : dto.SiparisTuru;
                var prefix = siparisTuru == "Alış" ? "ALS" : "SAT";
                var bugun = DateTime.Today;
                var tarihFormat = bugun.ToString("yyyyMMdd");
                var bugunkuSiparisSayisi = _context.Siparisler.Count(s => s.SiparisTarihi.Date == bugun);
                var otomatikSiparisNo = $"{prefix}-{tarihFormat}-{(bugunkuSiparisSayisi + 1).ToString("D4")}";

                // Sipariş Başlığı (Master)
                var siparis = new Siparis
                {
                    SiparisId = Guid.NewGuid(),
                    CariId = dto.CariId,
                    DepoId = dto.DepoId,
                    SiparisNumarasi = otomatikSiparisNo,
                    SiparisTuru = siparisTuru,
                    SiparisTarihi = dto.SiparisTarihi,
                    VadeTarihi = dto.VadeTarihi,
                    OlusturulmaTarihi = DateTime.UtcNow,
                    ToplamTutar = dto.Detaylar.Sum(x => x.Miktar * x.BirimFiyat),
                    Durum = "Bekliyor",
                    IsDeleted = false
                };

                _context.Siparisler.Add(siparis);

                // Sipariş Detay Satırlarını (Detail) Döngüyle Ekleme
                foreach (var detayDto in dto.Detaylar)
                {
                    // Eğer Satış Siparişi ise: Depodaki stok kontrol edilir ve rezerve edilir/düşülür
                    if (siparisTuru == "Satış")
                    {
                        var stok = _context.Stoklar.FirstOrDefault(s => s.DepoId == dto.DepoId && s.UrunId == detayDto.UrunId && !s.IsDeleted);
                        if (stok == null)
                        {
                            var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == detayDto.UrunId);
                            string urunAdi = urun?.UrunAdi ?? "Bilinmeyen Ürün";
                            throw new Exception($"'{urunAdi}' ürünü için seçilen depoda aktif stok kaydı bulunamadı!");
                        }

                        if (stok.Miktar < detayDto.Miktar)
                        {
                            var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == detayDto.UrunId);
                            string urunAdi = urun?.UrunAdi ?? "Bilinmeyen Ürün";
                            throw new Exception($"Yetersiz stok! '{urunAdi}' ürünü için depoda sadece {stok.Miktar} adet mevcut (Sipariş: {detayDto.Miktar} adet).");
                        }

                        stok.Miktar -= detayDto.Miktar;
                        stok.SonGuncellenmeTarihi = DateTime.UtcNow;
                    }

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

                _context.SaveChanges();
                transaction.Commit();

                return Json(ResultDto.Success($"{(siparisTuru == "Alış" ? "Alış" : "Satış")} siparişi başarıyla oluşturuldu. (No: {otomatikSiparisNo})"));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(ResultDto.Failure("Sipariş oluşturulurken bir hata meydana geldi: " + ex.Message));
            }
        }

        // 4. Sipariş Durumunu Güncelleyen (Tamamla / İptal Et) POST Metodu
        [HttpPost]
        public IActionResult SiparisDurumGuncelle(Guid id, string yeniDurum)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var siparis = _context.Siparisler.FirstOrDefault(s => s.SiparisId == id);
                if (siparis == null)
                {
                    return Json(ResultDto.Failure("Sipariş bulunamadı!"));
                }

                if (siparis.Durum == "İptal Edildi" && yeniDurum != "İptal Edildi")
                {
                    return Json(ResultDto.Failure("İptal edilmiş bir siparişin durumu değiştirilemez!"));
                }

                var detaylar = _context.SiparislerDetaylar.Where(d => d.SiparisId == id && !d.IsDeleted).ToList();

                // TAMAMLA İŞLEMİ
                if (yeniDurum == "Tamamlandı")
                {
                    if (siparis.Durum == "Tamamlandı")
                    {
                        return Json(ResultDto.Failure("Bu sipariş zaten tamamlanmış durumda!"));
                    }

                    // Eğer Alış Siparişi ise: Ürünler depoya ulaştığı için stoklara dahil edilir
                    if (siparis.SiparisTuru == "Alış" && siparis.DepoId.HasValue)
                    {
                        foreach (var detay in detaylar)
                        {
                            var stok = _context.Stoklar.FirstOrDefault(s => s.DepoId == siparis.DepoId.Value && s.UrunId == detay.UrunId);
                            if (stok != null)
                            {
                                stok.Miktar += detay.Miktar;
                                stok.IsDeleted = false;
                                stok.SonGuncellenmeTarihi = DateTime.UtcNow;
                            }
                            else
                            {
                                var yeniStok = new Stok
                                {
                                    StokId = Guid.NewGuid(),
                                    DepoId = siparis.DepoId.Value,
                                    UrunId = detay.UrunId,
                                    Miktar = detay.Miktar,
                                    IsDeleted = false,
                                    OlusturulmaTarihi = DateTime.UtcNow,
                                    SonGuncellenmeTarihi = DateTime.UtcNow
                                };
                                _context.Stoklar.Add(yeniStok);
                            }
                        }
                    }

                    siparis.Durum = "Tamamlandı";
                    _context.SaveChanges();
                    transaction.Commit();

                    string ekMesaj = siparis.SiparisTuru == "Alış" 
                        ? " Mal kabulü yapıldı ve ürünler ilgili depoya eklendi." 
                        : " Satış siparişi başarıyla tamamlandı.";

                    return Json(ResultDto.Success("Sipariş tamamlandı olarak işaretlendi!" + ekMesaj));
                }

                // İPTAL ETME İŞLEMİ
                if (yeniDurum == "İptal Edildi")
                {
                    if (siparis.IsDeleted || siparis.Durum == "İptal Edildi")
                    {
                        return Json(ResultDto.Failure("Bu sipariş zaten iptal edilmiş!"));
                    }

                    // Eğer Satış Siparişi ise: Rezerve edilen ürünler depoya geri iade edilir
                    if (siparis.SiparisTuru == "Satış" && siparis.DepoId.HasValue)
                    {
                        foreach (var detay in detaylar)
                        {
                            var stok = _context.Stoklar.FirstOrDefault(s => s.DepoId == siparis.DepoId.Value && s.UrunId == detay.UrunId);
                            if (stok != null)
                            {
                                stok.Miktar += detay.Miktar;
                                stok.IsDeleted = false;
                                stok.SonGuncellenmeTarihi = DateTime.UtcNow;
                            }
                            else
                            {
                                var yeniStok = new Stok
                                {
                                    StokId = Guid.NewGuid(),
                                    DepoId = siparis.DepoId.Value,
                                    UrunId = detay.UrunId,
                                    Miktar = detay.Miktar,
                                    IsDeleted = false,
                                    OlusturulmaTarihi = DateTime.UtcNow,
                                    SonGuncellenmeTarihi = DateTime.UtcNow
                                };
                                _context.Stoklar.Add(yeniStok);
                            }
                            detay.IsDeleted = true;
                        }
                    }
                    else if (siparis.SiparisTuru == "Alış" && siparis.Durum == "Tamamlandı" && siparis.DepoId.HasValue)
                    {
                        // Daha önce tamamlanıp depoya giren alış siparişi iptal ediliyorsa stok düşülür
                        foreach (var detay in detaylar)
                        {
                            var stok = _context.Stoklar.FirstOrDefault(s => s.DepoId == siparis.DepoId.Value && s.UrunId == detay.UrunId);
                            if (stok != null)
                            {
                                stok.Miktar = Math.Max(0, stok.Miktar - detay.Miktar);
                                stok.SonGuncellenmeTarihi = DateTime.UtcNow;
                            }
                            detay.IsDeleted = true;
                        }
                    }
                    else
                    {
                        foreach (var detay in detaylar)
                        {
                            detay.IsDeleted = true;
                        }
                    }

                    siparis.Durum = "İptal Edildi";
                    siparis.IsDeleted = true;

                    _context.SaveChanges();
                    transaction.Commit();

                    return Json(ResultDto.Success("Sipariş başarıyla iptal edildi ve stok hareketleri güncellendi!"));
                }

                // Diğer genel durum güncellemeleri
                siparis.Durum = yeniDurum;
                _context.SaveChanges();
                transaction.Commit();

                return Json(ResultDto.Success($"Sipariş durumu '{yeniDurum}' olarak güncellendi."));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(ResultDto.Failure("Sipariş durumu güncellenirken hata oluştu: " + ex.Message));
            }
        }

        // 5. Siparişi İptal Eden (Geriye uyumluluk için SiparisSil)
        [HttpPost]
        public IActionResult SiparisSil(Guid id)
        {
            return SiparisDurumGuncelle(id, "İptal Edildi");
        }

        // 6. Düzenleme için tek bir siparişin başlık bilgilerini getiren GET metodu
        [HttpGet]
        public IActionResult SiparisGetir(Guid id)
        {
            try
            {
                var siparis = (from s in _context.Siparisler
                               join c in _context.Cariler on s.CariId equals c.Id
                               where s.SiparisId == id && !s.IsDeleted
                               select new
                               {
                                   SiparisId = s.SiparisId,
                                   SiparisNumarasi = s.SiparisNumarasi,
                                   CariId = s.CariId,
                                   CariAdi = c.Ad,
                                   DepoId = s.DepoId,
                                   SiparisTuru = string.IsNullOrEmpty(s.SiparisTuru) ? "Satış" : s.SiparisTuru,
                                   SiparisTarihi = s.SiparisTarihi.ToString("yyyy-MM-dd"),
                                   VadeTarihi = s.VadeTarihi.HasValue ? s.VadeTarihi.Value.ToString("yyyy-MM-dd") : "",
                                   Durum = s.Durum
                               }).FirstOrDefault();

                if (siparis == null)
                {
                    return Json(ResultDto.Failure("Sipariş bulunamadı!"));
                }

                return Json(ResultDto<object>.Success(siparis, "Sipariş bilgileri başarıyla getirildi."));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Sipariş bilgileri yüklenirken hata oluştu: " + ex.Message));
            }
        }

        // 7. Sipariş Başlık Bilgilerini Güncelleyen POST Metodu
        [HttpPost]
        public IActionResult SiparisGuncelle([FromBody] SiparisGuncelleDto dto)
        {
            try
            {
                var siparis = _context.Siparisler.FirstOrDefault(s => s.SiparisId == dto.SiparisId && !s.IsDeleted);
                if (siparis == null)
                {
                    return Json(ResultDto.Failure("Güncellenecek sipariş bulunamadı!"));
                }

                if (siparis.Durum == "İptal Edildi")
                {
                    return Json(ResultDto.Failure("İptal edilmiş bir sipariş düzenlenemez!"));
                }

                if (dto.CariId != Guid.Empty)
                {
                    siparis.CariId = dto.CariId;
                }

                if (dto.DepoId.HasValue)
                {
                    siparis.DepoId = dto.DepoId.Value;
                }

                if (!string.IsNullOrEmpty(dto.SiparisTuru))
                {
                    siparis.SiparisTuru = dto.SiparisTuru;
                }

                siparis.SiparisTarihi = dto.SiparisTarihi;
                siparis.VadeTarihi = dto.VadeTarihi;

                _context.SaveChanges();

                return Json(ResultDto.Success("Sipariş başlık ve tarih bilgileri başarıyla güncellendi!"));
            }
            catch (Exception ex)
            {
                return Json(ResultDto.Failure("Sipariş güncellenirken hata oluştu: " + ex.Message));
            }
        }

        // 8. Tek bir siparişin kalem ve özet detaylarını getiren GET metodu (Modal ve PDF için)
        [HttpGet]
        public IActionResult SiparisDetayGetir(Guid id)
        {
            try
            {
                var siparis = (from s in _context.Siparisler
                               join c in _context.Cariler on s.CariId equals c.Id
                               join d in _context.Depolar on s.DepoId equals d.DepoId into depogroup
                               from d in depogroup.DefaultIfEmpty()
                               where s.SiparisId == id
                               select new
                               {
                                   SiparisId = s.SiparisId,
                                   SiparisNumarasi = s.SiparisNumarasi,
                                   CariId = s.CariId,
                                   CariAdi = c.Ad,
                                   CariKodu = c.CariKodu,
                                   CariTuru = c.CariTuru,
                                   DepoAdi = d != null ? d.DepoAdi : "Merkez Depo",
                                   SiparisTuru = string.IsNullOrEmpty(s.SiparisTuru) ? "Satış" : s.SiparisTuru,
                                   SiparisTarihi = s.SiparisTarihi.ToString("dd.MM.yyyy"),
                                   VadeTarihi = s.VadeTarihi.HasValue ? s.VadeTarihi.Value.ToString("dd.MM.yyyy") : "-",
                                   OlusturulmaTarihi = s.OlusturulmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                                   ToplamTutar = s.ToplamTutar,
                                   Durum = s.Durum
                               }).FirstOrDefault();

                if (siparis == null)
                {
                    return Json(ResultDto.Failure("Sipariş bulunamadı!"));
                }

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

        // 9. Fatura / İrsaliye Yazdırma (PDF) Görünümü
        [HttpGet]
        public IActionResult FaturaPdf(Guid id)
        {
            try
            {
                var fatura = (from s in _context.Siparisler
                               join c in _context.Cariler on s.CariId equals c.Id
                               join d in _context.Depolar on s.DepoId equals d.DepoId into depogroup
                               from d in depogroup.DefaultIfEmpty()
                               where s.SiparisId == id && !s.IsDeleted
                               select new FaturaViewModel
                               {
                                   SiparisNumarasi = s.SiparisNumarasi,
                                   CariAdi = c.Ad,
                                   CariKodu = c.CariKodu,
                                   CariTuru = c.CariTuru,
                                   DepoAdi = d != null ? d.DepoAdi : "Merkez Depo",
                                   SiparisTuru = string.IsNullOrEmpty(s.SiparisTuru) ? "Satış" : s.SiparisTuru,
                                   SiparisTarihi = s.SiparisTarihi,
                                   VadeTarihi = s.VadeTarihi,
                                   ToplamTutar = s.ToplamTutar,
                                   Durum = s.Durum
                               }).FirstOrDefault();

                if (fatura == null)
                {
                    return Content("Hata: Sipariş bulunamadı!");
                }

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
        public string DepoAdi { get; set; } = string.Empty;
        public string SiparisTuru { get; set; } = "Satış";
        public DateTime SiparisTarihi { get; set; }
        public DateTime? VadeTarihi { get; set; }
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
