using System;
using System.Collections.Generic;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;

namespace UretimTakip.Business.services
{
    public interface IStokService
    {
        ResultDto<object> StoklariListele(Guid? depoId, string? aramaKelimesi = null, bool arsivdekiler = false);
        ResultDto<object> GetUrunler();
        ResultDto<object> GetDepolar();
        ResultDto StokEkle(Stok yeniStok);
        ResultDto StokGuncelle(Guid id, int yeniMiktar);
        ResultDto StokSil(Guid id);
        ResultDto StokGeriYukle(Guid id);
        ResultDto StokDus(Guid urunId, Guid depoId, int miktar);
    }
}
