using System;
namespace ChatApi
{
    public class Authentication
    {
        public string AccessToken { get; set; }
        public string Scope { get; set; }
        public string ExpiresIn { get; set; }
        public string TokenType { get; set; }

        public Authentication()
        {
        }
    }
}
