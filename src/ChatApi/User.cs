using ChatApi.Infrastructure;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using ChatApi.Utils;
using System;
using ChatApi.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApi
{
    public class User
    {
        [Key]
        [Required]
        [MaxLength(10)]
        public string Username { get; set; }

        [Required]
        [MaxLength(64)]
        public string Password { get; set; }

        [NotMapped]
        public ChatContext DbContext { get; set; }

        [NotMapped]
        public TokenCredentials TokenCredentials { get; set; }

        public User() { }

        public User(string username, string password)
        {
            Username = username;
            Password = password.GetHashSha256();
        }

        public bool AreCredentialsValid()
        {
            var dbUser = DbContext.Users.FirstOrDefault(user => user.Username == Username);

            return dbUser != null && IsPasswordValid(dbUser.Password);
        }

        public bool IsPasswordValid(string storedPassword)
        {
            return Password.GetHashSha256().Equals(storedPassword);
        }

        public Authentication Authenticate()
        {
            var expirationTime = DateTime.UtcNow
                .AddDays(Convert.ToInt32(TokenCredentials.ExpireInDays));

            return new Authentication
            {
                AccessToken = GenerateAccessToken(expirationTime),
                TokenType = JwtBearerDefaults.AuthenticationScheme
            };
        }

        public string GenerateAccessToken(DateTime expirationTime)
        {
            var symmetricKey = Convert.FromBase64String(TokenCredentials.HmacSecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, Username)
                }),

                Expires = expirationTime,

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(symmetricKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }
    }
}
