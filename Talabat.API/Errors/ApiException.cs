namespace Talabat.API.Errors
{
    public class ApiException : ApiResponse
    {
        public string Details { get; }
        public ApiException(int statueCode, string message = null, string details = null) 
            : base(statueCode, message)
        {
            Details = details;
        }

    }
}
