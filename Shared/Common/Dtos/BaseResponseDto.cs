using System;
using System.Collections.Generic;

namespace Common.Dtos
{
    public class BaseResponse<T>
    {
        /// <summary>
        /// Trạng thái của kết quả (thành công hay thất bại).
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Dữ liệu trả về khi thành công.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Danh sách các thông báo lỗi nếu có.
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Thời gian xử lý yêu cầu.
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Mã trạng thái HTTP.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Tạo phản hồi thành công với dữ liệu và mã trạng thái.
        /// </summary>
        /// <param name="data">Dữ liệu cần trả về.</param>
        /// <param name="statusCode">Mã trạng thái HTTP.</param>
        /// <returns>Phản hồi với trạng thái thành công và dữ liệu kèm theo.</returns>
        public static BaseResponse<T> Success(T data, int statusCode = 200)
        {
            return new BaseResponse<T>
            {
                IsSuccess = true,
                Data = data,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Tạo phản hồi thất bại với danh sách lỗi và mã trạng thái.
        /// </summary>
        /// <param name="errors">Danh sách lỗi.</param>
        /// <param name="statusCode">Mã trạng thái HTTP.</param>
        /// <returns>Phản hồi với trạng thái thất bại và danh sách lỗi kèm theo.</returns>
        public static BaseResponse<T> Failure(List<string> errors, int statusCode = 400)
        {
            return new BaseResponse<T>
            {
                IsSuccess = false,
                Errors = errors,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Tạo phản hồi thất bại với một lỗi duy nhất và mã trạng thái.
        /// </summary>
        /// <param name="error">Lỗi cần thông báo.</param>
        /// <param name="statusCode">Mã trạng thái HTTP.</param>
        /// <returns>Phản hồi với trạng thái thất bại và lỗi kèm theo.</returns>
        public static BaseResponse<T> Failure(string error, int statusCode = 400)
        {
            return new BaseResponse<T>
            {
                IsSuccess = false,
                Errors = new List<string> { error },
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Tạo phản hồi thành công với dữ liệu và mã trạng thái.
        /// </summary>
        /// <param name="data">Dữ liệu cần trả về.</param>
        /// <param name="statusCode">Mã trạng thái HTTP.</param>
        /// <returns>Phản hồi với trạng thái thành công và dữ liệu kèm theo.</returns>
        public static BaseResponse<T> Accepted(T data, int statusCode = 202)
        {
            return new BaseResponse<T>
            {
                IsSuccess = true,
                Data = data,
                StatusCode = statusCode
            };
        }
    }
}
