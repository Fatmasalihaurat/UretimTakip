using System;
using UretimTakip.core.DTOs;
using UretimTakip.core.Entities;

namespace UretimTakip.Business.services
{
    public interface IHedefService
    {
        ResultDto<object> HedefleriListele();
        ResultDto<object> HedefGetir(Guid id);
        ResultDto HedefEkle(Hedef yeniHedef);
        ResultDto HedefGuncelle(Hedef guncelHedef);
        ResultDto HedefSil(Guid id);
    }
}
