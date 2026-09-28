using System;
using System.Collections.Generic;
using System.Linq;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;
using UretimTakip.DataAccess.Context;

namespace UretimTakip.Business.services
{
    public class DepoService : IDepoService
    {
        private readonly ApplicationDbContext _context;

        public DepoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ResultDto<List<Depo>> DepolariListele(string? aramaParametresi = null)
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
                return ResultDto<List<Depo>>.Success(depolar, "Depolar başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<List<Depo>>.Failure("Depolar listelenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto<Depo> DepoGetir(Guid id)
        {
            try
            {
                var depo = _context.Depolar.FirstOrDefault(d => d.DepoId == id && !d.IsArchived);
                if (depo == null)
                {
                    return ResultDto<Depo>.Failure("Depo bulunamadı!");
                }
                return ResultDto<Depo>.Success(depo, "Depo bilgileri getirildi.");
            }
            catch (Exception ex)
            {
                return ResultDto<Depo>.Failure("Depo getirilirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto DepoEkle(Depo yeniDepo)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniDepo.Ad) || string.IsNullOrEmpty(yeniDepo.DepoKodu))
                {
                    return ResultDto.Failure("Depo adı veya kodu boş bırakılamaz!");
                }

                yeniDepo.Id = Guid.NewGuid();
                yeniDepo.OlusturulmaTarihi = DateTime.Now;

                _context.Depolar.Add(yeniDepo);
                _context.SaveChanges();

                return ResultDto.Success("Yeni depo başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Depo oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto DepoGuncelle(Depo guncelDepo)
        {
            try
            {
                if (string.IsNullOrEmpty(guncelDepo.Ad) || string.IsNullOrEmpty(guncelDepo.DepoKodu))
                {
                    return ResultDto.Failure("Depo adı veya kodu boş bırakılamaz!");
                }

                var depo = _context.Depolar.FirstOrDefault(d => d.DepoId == guncelDepo.Id);
                if (depo == null)
                {
                    return ResultDto.Failure("Güncellenecek depo bulunamadı!");
                }

                depo.DepoAdi = guncelDepo.Ad;
                depo.DepoKodu = guncelDepo.DepoKodu;
                depo.SorumluKisi = guncelDepo.SorumluKisi;
                depo.Konum = guncelDepo.Konum;
                depo.IrtibatBilgisi = guncelDepo.IrtibatBilgisi;
                depo.Adres = guncelDepo.Adres;

                _context.SaveChanges();

                return ResultDto.Success("Depo başarıyla güncellendi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Depo güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public ResultDto DepoSil(Guid id)
        {
            try
            {
                var depo = _context.Depolar.FirstOrDefault(d => d.DepoId == id);
                if (depo == null)
                {
                    return ResultDto.Failure("Depo bulunamadı!");
                }

                depo.IsArchived = true;
                _context.SaveChanges();

                return ResultDto.Success("Depo başarıyla silindi!");
            }
            catch (Exception ex)
            {
                return ResultDto.Failure("Depo silinirken hata oluştu: " + ex.Message);
            }
        }
    }
}
