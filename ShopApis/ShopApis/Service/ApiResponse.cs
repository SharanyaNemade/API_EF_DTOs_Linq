using System.Text.Json.Serialization;

namespace ShopApis.Service
{


    public class ApiError
    {
        public string Code { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;
    }


    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public ApiError? Error { get; set; }


        // Left out of the JSON when null (so the error response matches your format)
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Metadata { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalNumberRecord { get; set; }



        public static ApiResponse<T> SuccessResponse(T data, string message, int? totalRecords = null) => new()
        {
            Success = true,
            Message = message,
            Data = data,
            Error = null,
            Metadata = new { },
            TotalNumberRecord = totalRecords
        };


        public static ApiResponse<T> Fail(string message, string code, string details) => new()
        {
            Success = false,
            Message = message,
            Data = default,
            Error = new ApiError { Code = code, Details = details }
        };
    }
}
