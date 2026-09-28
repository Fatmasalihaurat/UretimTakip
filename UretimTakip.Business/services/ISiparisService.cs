using System;
using UretimTakip.core.DTOs;

namespace UretimTakip.Business.services
{
    public interface ISiparisService
    {
        ResultDto<object> SiparisleriListele(string? aramaKelimesi = null, string? siparisTuru = null, string? durum = null);
        ResultDto SiparisOlustur(SiparisOlusturDto dto);
        ResultDto SiparisDurumGuncelle(Guid id, string yeniDurum);
        ResultDto SiparisSil(Guid id);
        ResultDto<object> SiparisGetir(Guid id);
        ResultDto SiparisGuncelle(SiparisGuncelleDto dto);
        ResultDto<object> SiparisDetayGetir(Guid id);
        ResultDto<FaturaDto> FaturaGetir(Guid id);
    }
}
