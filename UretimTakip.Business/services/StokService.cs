using System;
using System.Collections.Generic;
using System.Linq;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Business.services
{
    public class StokService : IStokService
    {
        private readonly ApplicationDbContext _context;

        public StokService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ResultDto<object> StoklariListele(Guid? depoId, string? aramaKelimesi = null, bool arsivdekiler = false)
        {
            try
            {
                var sorgu = from s in _context.Stoklar
                            join u in _context.Urunler on s.UrunId equals u.UrunId into urunGroup
                            from u in urunGroup.DefaultIfEmpty()
                            join d in _context.Depolar on s.DepoId equals d.DepoId into depoGroup
                            from d in depoGroup.DefaultIfEmpty()
                            where s.IsDeleted == arsivdekiler
                            select new { s, u, d };

                if (depoId.HasValue)
                {
                    sorgu = sorgu.Where(x => x.s.DepoId == depoId.Value);
                }

                if (!string.IsNullOrEmpty(aramaKelimesi))
                {
                    aramaKelimesi = aramaKelimesi.ToLower();
                    sorgu = sorgu.Where(x => (x.u != null && x.u.UrunAdi.ToLower().Contains(aramaKelimesi)) || 
                                             (x.s.StokKodu != null && x.s.StokKodu.ToLower().Contains(aramaKelimesi)) ||
                                             (x.d != null && x.d.DepoAdi.ToLower().Contains(aramaKelimesi)));
                }

                var stoklar = sorgu.ToList().Select(x => new
                {
                    StokId = x.s.StokId,
                    StokKodu = x.s.StokKodu,
                    UrunId = x.s.UrunId,
                    DepoId = x.s.DepoId,
                    UrunAdi = x.u != null ? x.u.UrunAdi : "Bilinmeyen / Silinmiş Ürün",
                    DepoAdi = x.d != null ? x.d.DepoAdi : "Bilinmeyen / Arşivlenmiş Depo",
                    Miktar = x.s.Miktar,
                    SonGuncellenmeTarihi = x.s.SonGuncellenmeTarihi.ToString("dd.MM.yyyy HH:mm"),
                    IsDeleted = x.s.IsDeleted,
                    SilinebilirMi = (x.s.Miktar == 0)
                }).ToList();

                return ResultDto<object>.Success(stoklar, "Stoklar başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Stoklar listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<object> GetUrunler()
        {
            try
            {
                var urunler = _context.Urunler
                    .Where(x => !x.IsDeleted)
                    .Select(x => new { x.UrunId, x.UrunAdi })
                    .ToList();
                return ResultDto<object>.Success(urunler, "Ürünler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Ürünler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<object> GetDepolar()
        {
            try
            {
                var depolar = _context.Depolar
                    .Where(x => !x.IsArchived)
                    .Select(x => new { x.DepoId, x.DepoAdi })
                    .ToList();
                return ResultDto<object>.Success(depolar, "Depolar başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Depolar listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto StokEkle(Stok yeniStok)
        {
            try
            {
                if (yeniStok.UrunId == Guid.Empty)
                {
                    return ResultDto.Failure("Lütfen bir ürün seçin!");
                }
                if (yeniStok.DepoId == Guid.Empty)
                {
                    return ResultDto.Failure("Lütfen bir depo seçin!");
                }
                if (string.IsNullOrEmpty(yeniStok.StokKodu))
                {
                    return ResultDto.Failure("Stok kodu boş bırakılamaz!");
                }
                if (yeniStok.Miktar < 0)
                {
                    return ResultDto.Failure("Başlangıç stok miktarı 0 veya daha büyük olmalıdır!");
                }

                var mevcutStok = _context.Stoklar.FirstOrDefault(x => x.UrunId == yeniStok.UrunId && x.DepoId == yeniStok.DepoId);
                if (mevcutStok != null)
                {
                    if (!mevcutStok.IsDeleted)
                    {
                        return ResultDto.Failure("Bu ürün bu depoda zaten tanımlanmış! Lütfen mevcut stok miktarını güncelleyin.");
                    }
                    else
                    {
                        mevcutStok.IsDeleted = false;
                        mevcutStok.Miktar = yeniStok.Miktar;
                        if (!string.IsNullOrEmpty(yeniStok.StokKodu))
                        {
                            mevcutStok.StokKodu = yeniStok.StokKodu;
                        }
                        mevcutStok.SonGuncellenmeTarihi = DateTime.UtcNow;

                        _context.SaveChanges();
                        return ResultDto.Success("Daha önce arşivlenen stok kaydı yeniden aktifleştirildi ve güncellendi!");
                    }
                }

                yeniStok.StokId = Guid.NewGuid();
                yeniStok.OlusturulmaTarihi = DateTime.UtcNow;
                yeniStok.SonGuncellenmeTarihi = DateTime.UtcNow;

                _context.Stoklar.Add(yeniStok);
                _context.SaveChanges();

                return ResultDto.Success("Yeni stok kaydı başarıyla oluşturuldu!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Stok kaydı oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto StokGuncelle(Guid id, int yeniMiktar)
        {
            try
            {
                if (yeniMiktar < 0)
                {
                    return ResultDto.Failure("Stok miktarı 0'dan küçük olamaz!");
                }

                var stok = _context.Stoklar.FirstOrDefault(x => x.StokId == id);
                if (stok == null)
                {
                    return ResultDto.Failure("Stok kaydı bulunamadı!");
                }

                stok.Miktar = yeniMiktar;
                stok.SonGuncellenmeTarihi = DateTime.UtcNow;

                _context.SaveChanges();

                if (yeniMiktar == 0)
                {
                    return ResultDto.Success("Stok miktarı 0 olarak güncellendi. Dilerseniz bu kaydı 'Arşivle (Sil)' butonu ile arşive kaldırabilirsiniz.");
                }

                return ResultDto.Success("Stok miktarı başarıyla güncellendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Stok güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto StokSil(Guid id)
        {
            try
            {
                var stok = _context.Stoklar.FirstOrDefault(x => x.StokId == id);
                if (stok == null)
                {
                    return ResultDto.Failure("Stok kaydı bulunamadı!");
                }

                if (stok.Miktar > 0)
                {
                    return ResultDto.Failure($"Miktarı {stok.Miktar} olan bir stok kaydı silinemez! Sadece stok miktarı 0 olan kayıtlar arşivlenebilir. Lütfen önce 'Güncelle' butonu ile stoğu sıfırlayın.");
                }

                stok.IsDeleted = true;
                stok.Miktar = 0;
                stok.SonGuncellenmeTarihi = DateTime.UtcNow;
                _context.SaveChanges();

                return ResultDto.Success("Stok kaydı başarıyla arşivlendi! Geçmiş sipariş ve hareket kayıtlarının korunması için veritabanında arşiv olarak saklanmaya devam edecektir.");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Stok silinirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto StokGeriYukle(Guid id)
        {
            try
            {
                var stok = _context.Stoklar.FirstOrDefault(x => x.StokId == id);
                if (stok == null)
                {
                    return ResultDto.Failure("Stok kaydı bulunamadı!");
                }

                stok.IsDeleted = false;
                stok.SonGuncellenmeTarihi = DateTime.UtcNow;
                _context.SaveChanges();

                return ResultDto.Success("Stok kaydı başarıyla arşivden çıkarıldı ve tekrar aktif stoklar listesine dahil edildi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Stok geri yüklenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto StokDus(Guid urunId, Guid depoId, int miktar)
        {
            try
            {
                var mevcutStok = _context.Stoklar.FirstOrDefault(x => x.UrunId == urunId && x.DepoId == depoId && !x.IsDeleted);

                if (mevcutStok == null || mevcutStok.Miktar < miktar)
                {
                    return ResultDto.Failure("İşlem Reddi: Yeterli stok yok!");
                }

                mevcutStok.Miktar -= miktar;
                mevcutStok.SonGuncellenmeTarihi = DateTime.UtcNow;

                _context.SaveChanges();
                return ResultDto.Success("Stok düşümü yapıldı.");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("HATA! : " + ex.Message);
            }
        }
    }
}
