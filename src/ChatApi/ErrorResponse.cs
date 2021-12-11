using System;
namespace ChatApi
{
    public class ErrorResponse
    {
        public string Error { get; set; }
        public string ErrorDescription { get; set; }

        public ErrorResponse()
        {
        }
    }
}
