using System;
using System.Collections.Generic;
using System.Linq;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Business.services
{
    public class SiparisService : ISiparisService
    {
        private readonly ApplicationDbContext _context;

        public SiparisService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ResultDto<object> SiparisleriListele(string? aramaKelimesi = null, string? siparisTuru = null, string? durum = null)
        {
            try
            {
                var query = from s in _context.Siparisler
                            join c in _context.Cariler on s.CariId equals c.Id
                            join d in _context.Depolar on s.DepoId equals d.DepoId into depogroup
                            from d in depogroup.DefaultIfEmpty()
                            select new { s, c, d };

                if (!string.IsNullOrEmpty(siparisTuru))
                {
                    query = query.Where(x => x.s.SiparisTuru.ToLower() == siparisTuru.ToLower());
                }

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

                return ResultDto<object>.Success(siparisler, "Siparişler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Siparişler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto SiparisOlustur(SiparisOlusturDto dto)
        {
            if (dto == null)
            {
                return ResultDto.Failure("Sipariş verisi boş olamaz.");
            }

            if (dto.Detaylar == null || !dto.Detaylar.Any())
            {
                return ResultDto.Failure("Sipariş oluşturabilmek için en az bir ürün eklemelisiniz.");
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

                foreach (var detayDto in dto.Detaylar)
                {
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

                return ResultDto.Success($"{(siparisTuru == "Alış" ? "Alış" : "Satış")} siparişi başarıyla oluşturuldu. (No: {otomatikSiparisNo})");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ResultDto.Failure("Sipariş oluşturulurken bir hata meydana geldi: " + ex.Message);
            }
        }

        public ResultDto SiparisDurumGuncelle(Guid id, string yeniDurum)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var siparis = _context.Siparisler.FirstOrDefault(s => s.SiparisId == id);
                if (siparis == null)
                {
                    return ResultDto.Failure("Sipariş bulunamadı!");
                }

                if (siparis.Durum == "İptal Edildi" && yeniDurum != "İptal Edildi")
                {
                    return ResultDto.Failure("İptal edilmiş bir siparişin durumu değiştirilemez!");
                }

                var detaylar = _context.SiparislerDetaylar.Where(d => d.SiparisId == id && !d.IsDeleted).ToList();

                if (yeniDurum == "Tamamlandı")
                {
                    if (siparis.Durum == "Tamamlandı")
                    {
                        return ResultDto.Failure("Bu sipariş zaten tamamlanmış durumda!");
                    }

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

                    return ResultDto.Success("Sipariş tamamlandı olarak işaretlendi!" + ekMesaj);
                }

                if (yeniDurum == "İptal Edildi")
                {
                    if (siparis.IsDeleted || siparis.Durum == "İptal Edildi")
                    {
                        return ResultDto.Failure("Bu sipariş zaten iptal edilmiş!");
                    }

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

                    return ResultDto.Success("Sipariş başarıyla iptal edildi ve stok hareketleri güncellendi!");
                }

                siparis.Durum = yeniDurum;
                _context.SaveChanges();
                transaction.Commit();

                return ResultDto.Success($"Sipariş durumu '{yeniDurum}' olarak güncellendi.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ResultDto.Failure("Sipariş durumu güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto SiparisSil(Guid id)
        {
            return SiparisDurumGuncelle(id, "İptal Edildi");
        }

        public ResultDto<object> SiparisGetir(Guid id)
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
                    return ResultDto<object>.Failure("Sipariş bulunamadı!");
                }

                return ResultDto<object>.Success(siparis, "Sipariş bilgileri başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Sipariş bilgileri yüklenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto SiparisGuncelle(SiparisGuncelleDto dto)
        {
            try
            {
                var siparis = _context.Siparisler.FirstOrDefault(s => s.SiparisId == dto.SiparisId && !s.IsDeleted);
                if (siparis == null)
                {
                    return ResultDto.Failure("Güncellenecek sipariş bulunamadı!");
                }

                if (siparis.Durum == "İptal Edildi")
                {
                    return ResultDto.Failure("İptal edilmiş bir sipariş düzenlenemez!");
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

                return ResultDto.Success("Sipariş başlık ve tarih bilgileri başarıyla güncellendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Sipariş güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<object> SiparisDetayGetir(Guid id)
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
                                   Durum = s.Durum,
                                   IsDeleted = s.IsDeleted
                               }).FirstOrDefault();

                if (siparis == null)
                {
                    return ResultDto<object>.Failure("Sipariş bulunamadı!");
                }

                var detaylar = (from sd in _context.SiparislerDetaylar
                                join u in _context.Urunler on sd.UrunId equals u.UrunId
                                where sd.SiparisId == id && !sd.IsDeleted
                                select new
                                {
                                    UrunId = sd.UrunId,
                                    UrunKodu = u.SistemUrunKodu,
                                    UrunAdi = u.UrunAdi,
                                    Miktar = sd.Miktar,
                                    BirimFiyat = sd.BirimFiyat,
                                    SatirToplam = sd.Miktar * sd.BirimFiyat
                                }).ToList();

                return ResultDto<object>.Success(new { siparis, detaylar }, "Sipariş detayları başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Sipariş detayları yüklenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<FaturaDto> FaturaGetir(Guid id)
        {
            try
            {
                var fatura = (from s in _context.Siparisler
                               join c in _context.Cariler on s.CariId equals c.Id
                               join d in _context.Depolar on s.DepoId equals d.DepoId into depogroup
                               from d in depogroup.DefaultIfEmpty()
                               where s.SiparisId == id && !s.IsDeleted
                               select new FaturaDto
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
                    return ResultDto<FaturaDto>.Failure("Sipariş bulunamadı!");
                }

                fatura.Detaylar = (from sd in _context.SiparislerDetaylar
                                   join u in _context.Urunler on sd.UrunId equals u.UrunId
                                   where sd.SiparisId == id && !sd.IsDeleted
                                   select new FaturaDetayDto
                                   {
                                       UrunKodu = u.SistemUrunKodu,
                                       UrunAdi = u.UrunAdi,
                                       Miktar = sd.Miktar,
                                       BirimFiyat = sd.BirimFiyat,
                                       SatirToplam = sd.Miktar * sd.BirimFiyat
                                   }).ToList();

                return ResultDto<FaturaDto>.Success(fatura, "Fatura bilgileri getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<FaturaDto>.Failure("Fatura yüklenirken hata oluştu: " + ex.Message);
            }
        }
    }
}
