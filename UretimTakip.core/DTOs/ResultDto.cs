using System;
using System.Collections.Generic;

namespace UretimTakip.core.DTOs
{
	public class ResultDto
	{
		// Ýþlemin baþarýlý olup olmadýðýný tutar (true/false)
		public bool IsSuccess { get; set; }

		// Kullanýcýya arayüzde göstereceðimiz mesaj ("Ürün baþarýyla eklendi" vb.)
		public string Message { get; set; } = string.Empty;

		// Eðer bir hata varsa, yazýlýmcýnýn görebileceði teknik detaylar veya validasyon hatalarý
		public List<string> Errors { get; set; } = new List<string>();

		// Baþarýlý durumlar için hýzlý nesne üretme metodu (Static Factory Method)
		public static ResultDto Success(string message = "Ýþlem baþarýyla tamamlandý.")
		{
			return new ResultDto { IsSuccess = true, Message = message };
		}

		// Baþarýsýz durumlar için hýzlý nesne üretme metodu
		public static ResultDto Failure(string message, List<string> errors = null)
		{
			return new ResultDto
			{
				IsSuccess = false,
				Message = message,
				Errors = errors ?? new List<string>()
			};
		}
	}

	// JENERÝK (GENERIC) RESULT DTO
	// Eðer backend'den sadece mesaj deðil, yanýnda bir de veri (örn: Ürün listesi) döneceksek bunu kullanýrýz.
	public class ResultDto<T> : ResultDto
	{
		// Ýçinde her türlü veri tipini (List<Urun>, Depo, Stok vb.) taþýyabilen jenerik alan
		public T Data { get; set; }

		public static ResultDto<T> Success(T data, string message = "Ýþlem baþarýyla tamamlandý.")
		{
			return new ResultDto<T> { IsSuccess = true, Message = message, Data = data };
		}

		public static new ResultDto<T> Failure(string message, List<string> errors = null)
		{
			return new ResultDto<T>
			{
				IsSuccess = false,
				Message = message,
				Errors = errors ?? new List<string>()
			};
		}
	}
}