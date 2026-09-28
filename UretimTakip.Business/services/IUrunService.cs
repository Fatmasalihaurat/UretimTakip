using System;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;

namespace UretimTakip.Business.services
{
    public interface IUrunService
    {
        ResultDto<object> UrunleriListele(string? aramaParametresi = null, bool arsivdekiler = false);
        ResultDto<object> UrunGetir(Guid id);
        ResultDto UrunEkle(Urun yeniUrun);
        ResultDto UrunGuncelle(Urun guncelUrun);
        ResultDto UrunSil(Guid id);
        ResultDto UrunGeriYukle(Guid id);
    }
}
