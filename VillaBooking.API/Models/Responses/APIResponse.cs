namespace VillaBooking.API.Models.Responses
{
    public class APIResponse<TData>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public TData? Data { get; set; }
        public object? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;


        public static APIResponse<TData> Create(bool success, int statusCode, string message, TData? data = default, object? errors = null)
        {
            return new APIResponse<TData>
            {
                Success = success,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = errors
            };
        }


        public static APIResponse<TData> Ok(TData data, string message) =>
            Create(true, StatusCodes.Status200OK, message, data);

        public static APIResponse<TData> CreatedAt(TData data, string message) =>
            Create(true, StatusCodes.Status201Created, message, data);

        public static APIResponse<TData> NoContent(string message = "Operation completed successfully") =>
            Create(true, StatusCodes.Status204NoContent, message);

        public static APIResponse<TData> NotFound(string message = "Resource not found") =>
            Create(false, StatusCodes.Status404NotFound, message);

        public static APIResponse<TData> BadRequest(string message, object? errors = null) =>
            Create(false, StatusCodes.Status400BadRequest, message, errors:errors);

        public static APIResponse<TData> Conflict(string message) =>
            Create(false, StatusCodes.Status409Conflict, message);


        public static APIResponse<TData> Error(int statusCode, string message, object? errors = null) =>
            Create(false, statusCode, message, errors:errors);
    }
}
