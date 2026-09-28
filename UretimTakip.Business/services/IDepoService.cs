using System;
using System.Collections.Generic;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;

namespace UretimTakip.Business.services
{
    public interface IDepoService
    {
        ResultDto<List<Depo>> DepolariListele(string? aramaParametresi = null);
        ResultDto<Depo> DepoGetir(Guid id);
        ResultDto DepoEkle(Depo yeniDepo);
        ResultDto DepoGuncelle(Depo guncelDepo);
        ResultDto DepoSil(Guid id);
    }
}
