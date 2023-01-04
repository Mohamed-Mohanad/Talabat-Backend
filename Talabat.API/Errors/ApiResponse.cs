namespace Talabat.API.Errors
{
    public class ApiResponse
    {
        public int StatueCode { get; set; }
        public string Message { get; set; }
        public ApiResponse(int statueCode, string message = null)
        {
            StatueCode = statueCode;
            Message = message ?? GetDefaultMessageForStatusCode(statueCode);
        }
        private string GetDefaultMessageForStatusCode(int statusCode)
            => statusCode switch
            {
                200 => "Success",
                400 => "A bad request, you have made",
                401 => "You are not authorized",
                404 => "Resources not found",
                500 => "server error",
                _ => null,
            };
    }
}
