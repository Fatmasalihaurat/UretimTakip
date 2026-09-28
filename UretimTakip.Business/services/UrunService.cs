using System;
using System.Collections.Generic;
using System.Linq;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Business.services
{
    public class UrunService : IUrunService
    {
        private readonly ApplicationDbContext _context;

        public UrunService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ResultDto<object> UrunleriListele(string? aramaParametresi = null, bool arsivdekiler = false)
        {
            try
            {
                var query = _context.Urunler.Where(x => x.IsDeleted == arsivdekiler);

                if (!string.IsNullOrEmpty(aramaParametresi))
                {
                    aramaParametresi = aramaParametresi.ToLower();
                    query = query.Where(x => x.UrunAdi.ToLower().Contains(aramaParametresi) || 
                                             x.SistemUrunKodu.ToLower().Contains(aramaParametresi));
                }

                var urunler = query.ToList();

                var aktifStoklar = _context.Stoklar
                    .Where(s => !s.IsDeleted)
                    .GroupBy(s => s.UrunId)
                    .Select(g => new { UrunId = g.Key, ToplamStok = g.Sum(x => x.Miktar) })
                    .ToDictionary(x => x.UrunId, x => x.ToplamStok);

                var sonuc = urunler.Select(u =>
                {
                    int toplamStok = aktifStoklar.TryGetValue(u.UrunId, out var stok) ? stok : 0;
                    return new
                    {
                        id = u.UrunId,
                        ad = u.UrunAdi,
                        urunKodu = u.SistemUrunKodu,
                        fiyat = u.Fiyat,
                        toplamStok = toplamStok,
                        silinebilirMi = (toplamStok == 0),
                        isDeleted = u.IsDeleted
                    };
                }).ToList();

                return ResultDto<object>.Success(sonuc, "Ürünler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Ürünler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<object> UrunGetir(Guid id)
        {
            try
            {
                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == id);

                if (urun == null)
                {
                    return ResultDto<object>.Failure("Ürün bulunamadı!");
                }

                return ResultDto<object>.Success(new
                {
                    id = urun.UrunId,
                    ad = urun.UrunAdi,
                    urunKodu = urun.SistemUrunKodu,
                    fiyat = urun.Fiyat
                }, "Ürün bilgileri getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<object>.Failure("Ürün getirilirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto UrunEkle(Urun yeniUrun)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniUrun.Ad) || string.IsNullOrEmpty(yeniUrun.UrunKodu))
                {
                    return ResultDto.Failure("Ürün adı veya kodu boş bırakılamaz!");
                }

                yeniUrun.Id = Guid.NewGuid();
                yeniUrun.CreatedDate = DateTime.UtcNow;
                yeniUrun.IsDeleted = false;

                _context.Urunler.Add(yeniUrun);
                _context.SaveChanges();

                return ResultDto.Success("Yeni ürün başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Ürün eklenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto UrunGuncelle(Urun guncelUrun)
        {
            try
            {
                if (string.IsNullOrEmpty(guncelUrun.Ad) || string.IsNullOrEmpty(guncelUrun.UrunKodu))
                {
                    return ResultDto.Failure("Ürün adı veya kodu boş bırakılamaz!");
                }

                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == guncelUrun.Id);

                if (urun == null)
                {
                    return ResultDto.Failure("Güncellenecek ürün bulunamadı!");
                }

                urun.UrunAdi = guncelUrun.Ad;
                urun.SistemUrunKodu = guncelUrun.UrunKodu;
                urun.Fiyat = guncelUrun.Fiyat;
                urun.GuncellenmeTarihi = DateTime.UtcNow;

                _context.SaveChanges();

                return ResultDto.Success("Ürün başarıyla güncellendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Ürün güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto UrunSil(Guid id)
        {
            try
            {
                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == id);

                if (urun == null)
                {
                    return ResultDto.Failure("Ürün bulunamadı!");
                }

                var aktifStoklar = _context.Stoklar.Where(s => s.UrunId == id && !s.IsDeleted).ToList();
                int toplamStok = aktifStoklar.Sum(s => s.Miktar);

                if (toplamStok > 0)
                {
                    return ResultDto.Failure($"Bu ürüne ait depolarda toplam {toplamStok} adet stok bulunmaktadır. Stoğu olan ürünler silinemez! Lütfen önce depolardaki stok miktarını sıfırlayın veya arşivleyin.");
                }

                urun.IsDeleted = true;
                urun.GuncellenmeTarihi = DateTime.UtcNow;

                foreach (var s in aktifStoklar)
                {
                    s.IsDeleted = true;
                    s.SonGuncellenmeTarihi = DateTime.UtcNow;
                }

                _context.SaveChanges();

                return ResultDto.Success("Ürün başarıyla silindi ve arşivlendi! Veritabanında kayıtlı kalmaya devam edecektir.");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Ürün silinirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto UrunGeriYukle(Guid id)
        {
            try
            {
                var urun = _context.Urunler.FirstOrDefault(u => u.UrunId == id);

                if (urun == null)
                {
                    return ResultDto.Failure("Ürün bulunamadı!");
                }

                urun.IsDeleted = false;
                urun.GuncellenmeTarihi = DateTime.UtcNow;

                _context.SaveChanges();

                return ResultDto.Success("Ürün başarıyla arşivden çıkarıldı ve tekrar aktif hale getirildi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Ürün geri yüklenirken hata oluştu: " + ex.Message);
            }
        }
    }
}
