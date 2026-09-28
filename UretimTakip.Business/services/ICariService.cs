using System;
using System.Collections.Generic;
using UretimTakip.core.DTOs;
using UretimTakip.core.Models;

namespace UretimTakip.Business.services
{
    public interface ICariService
    {
        ResultDto<List<Cari>> CarileriListele(string? aramaKelimesi = null, string? cariTuru = null);
        ResultDto<Cari> CariGetir(Guid id);
        ResultDto CariEkle(Cari yeniCari);
        ResultDto CariGuncelle(Cari guncelCari);
        ResultDto CariSil(Guid id);
    }
}
