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
using ChatApi.Domain.Notifications;
using System.Net;

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
        public ChatContext DbContext { get; set; }

        [NotMapped]
        public TokenCredentials TokenCredentials { get; set; }

        [NotMapped]
        public NotificationContext NotifyContext { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public Authentication Authenticate()
        {
            if (!AreCredentialsValid())
            {
                NotifyContext.AddNotification((int)HttpStatusCode.Unauthorized, "Access Denied");
                return null;
            }

            var expirationTime = DateTime.UtcNow
                .AddDays(Convert.ToInt32(TokenCredentials.ExpireInDays));

            return new Authentication
            {
                AccessToken = GenerateAccessToken(expirationTime),
                TokenType = JwtBearerDefaults.AuthenticationScheme
            };
        }

        public async Task Create()
        {
            var userAlreadyExists = DbContext.Users.Any(user => user.Username == Username);

            if (userAlreadyExists)
            {
                NotifyContext.AddNotification((int)HttpStatusCode.BadRequest, "User already exists");
                return;
            }

            Password = Password.GetHashSha256();
            await DbContext.Users.AddAsync(this);
            DbContext.SaveChanges();
        }

        public void Delete()
        {
            var userExists = DbContext.Users.Any(user => user.Username == Username);

            if (!userExists)
            {
                NotifyContext.AddNotification((int)HttpStatusCode.NotFound, "User does not exist");
                return;
            }

            DbContext.Users.Remove(this);
            DbContext.SaveChanges();
        }

        private bool AreCredentialsValid()
        {
            var dbUser = DbContext.Users.FirstOrDefault(user => user.Username == Username);

            return dbUser != null && Password.GetHashSha256().Equals(dbUser.Password);
        }

        private string GenerateAccessToken(DateTime expirationTime)
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
