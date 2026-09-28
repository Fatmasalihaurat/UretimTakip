using System;
using UretimTakip.core.DTOs;

namespace UretimTakip.Business.services
{
    public interface IBildirimService
    {
        ResultDto<object> BildirimleriListele();
        ResultDto<object> BildirimGetir(Guid id);
        ResultDto OkunduYap(Guid id);
        ResultDto TumunuOkunduYap();
        ResultDto BildirimSil(Guid id);
        ResultDto<int> OkunmamisSayisi();
        ResultDto ButceUyarisiOlustur();
    }
}
