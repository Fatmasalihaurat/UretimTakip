using System;
using System.Collections.Generic;
using System.Linq;
using UretimTakip.core.DTOs;
using UretimTakip.core.Models;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Business.services
{
    public class CariService : ICariService
    {
        private readonly ApplicationDbContext _context;

        public CariService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ResultDto<List<Cari>> CarileriListele(string? aramaKelimesi = null, string? cariTuru = null)
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
                return ResultDto<List<Cari>>.Success(cariler, "Cariler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<List<Cari>>.Failure("Cariler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<Cari> CariGetir(Guid id)
        {
            try
            {
                var cari = _context.Cariler.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
                if (cari == null)
                {
                    return ResultDto<Cari>.Failure("Cari bulunamadı!");
                }

                return ResultDto<Cari>.Success(cari, "Cari bilgileri başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<Cari>.Failure("Cari bilgisi getirilirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto CariEkle(Cari yeniCari)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniCari.Ad) || string.IsNullOrEmpty(yeniCari.CariKodu))
                {
                    return ResultDto.Failure("Cari adı veya kodu boş bırakılamaz!");
                }

                if (string.IsNullOrEmpty(yeniCari.CariTuru))
                {
                    return ResultDto.Failure("Lütfen cari türünü (Müşteri veya Tedarikçi) seçin!");
                }

                yeniCari.Id = Guid.NewGuid();
                yeniCari.Ad = yeniCari.Ad.Trim();
                yeniCari.CariKodu = yeniCari.CariKodu.Trim();
                yeniCari.IsDeleted = false;

                _context.Cariler.Add(yeniCari);
                _context.SaveChanges();

                return ResultDto.Success("Yeni cari başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Cari oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto CariGuncelle(Cari guncelCari)
        {
            try
            {
                if (string.IsNullOrEmpty(guncelCari.Ad) || string.IsNullOrEmpty(guncelCari.CariKodu))
                {
                    return ResultDto.Failure("Cari adı veya kodu boş bırakılamaz!");
                }

                if (string.IsNullOrEmpty(guncelCari.CariTuru))
                {
                    return ResultDto.Failure("Lütfen cari türünü seçin!");
                }

                var cari = _context.Cariler.FirstOrDefault(c => c.Id == guncelCari.Id && !c.IsDeleted);
                if (cari == null)
                {
                    return ResultDto.Failure("Güncellenecek cari bulunamadı!");
                }

                cari.Ad = guncelCari.Ad.Trim();
                cari.CariKodu = guncelCari.CariKodu.Trim();
                cari.CariTuru = guncelCari.CariTuru;

                _context.SaveChanges();

                return ResultDto.Success("Cari bilgileri başarıyla güncellendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Cari güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto CariSil(Guid id)
        {
            try
            {
                var cari = _context.Cariler.FirstOrDefault(c => c.Id == id);
                if (cari == null)
                {
                    return ResultDto.Failure("Cari bulunamadı!");
                }

                cari.IsDeleted = true;
                _context.SaveChanges();

                return ResultDto.Success("Cari kaydı başarıyla silindi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Cari silinirken hata oluştu: " + ex.Message);
            }
        }
    }
}
