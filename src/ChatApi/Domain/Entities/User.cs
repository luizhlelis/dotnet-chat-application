using ChatApi.Infrastructure;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatApi.Application.Settings;
using ChatApi.Domain;

namespace ChatApi
{
    public class User
    {
        [Key]
        [Required]
        [MaxLength(20)]
        public string Username { get; private set; }

        [Required]
        [MaxLength(64)]
        public string Password { get; private set; }

        [NotMapped]
        [JsonIgnore]
        public ChatContext DbContext { get; set; }

        [NotMapped]
        [JsonIgnore]
        public TokenCredentials TokenCredentials { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public async Task<string> Create()
        {
            var userAlreadyExists = DbContext.Users.Any(user => user.Username == Username);

            if (!userAlreadyExists)
            {
                await DbContext.Users.AddAsync(this);
                DbContext.SaveChanges();
                return string.Empty;
            }
            else
                return "User already exists";
        }

        public string Delete()
        {
            if (!DbContext.Users.Any(user => user.Username == Username))
            {
                DbContext.Users.Remove(this);
                DbContext.SaveChanges();
                return string.Empty;
            }
            else
                return "User doesn't exist";
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

                Audience = TokenCredentials.Audience,
                Issuer = TokenCredentials.Issuer,

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
