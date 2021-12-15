namespace ChatApi.Application.Settings
{
    public class TokenCredentials
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string ExpireInDays { get; set; }
        public string HmacSecretKey { get; set; }
    }
}
