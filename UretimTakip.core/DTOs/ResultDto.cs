using System;
using System.Collections.Generic;

namespace UretimTakip.core.DTOs
{
	public class ResultDto
	{
		public bool IsSuccess { get; set; }

		public string Message { get; set; } = string.Empty;

		public List<string> Errors { get; set; } = new List<string>();

		public static ResultDto Success(string message = "İşlem başarıyla tamamlandı.")
		{
			return new ResultDto { IsSuccess = true, Message = message };
		}

		public static ResultDto Failure(string message, List<string>? errors = null)
		{
			return new ResultDto
			{
				IsSuccess = false,
				Message = message,
				Errors = errors ?? new List<string>()
			};
		}
	}

	public class ResultDto<T> : ResultDto
	{
		public T? Data { get; set; }

		public static ResultDto<T> Success(T data, string message = "İşlem başarıyla tamamlandı.")
		{
			return new ResultDto<T> { IsSuccess = true, Message = message, Data = data };
		}

		public static new ResultDto<T> Failure(string message, List<string>? errors = null)
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
