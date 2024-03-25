using Azure.Core;
using Hair_saloon.Data;
using Hair_saloon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Hair_saloon.Tokens {
    public class Jwt {

        private IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HairSaloonContext _context;

        public Jwt(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, HairSaloonContext context )
        {
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public string GenerateJWT(UserLoginDto user) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var audience = _config["Jwt:Audience"];
            var issuer = _config["Jwt:Issuer"];
            TimeZoneInfo croatiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime expiresLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, croatiaTimeZone).AddMinutes(1);

            string username = user.Username;
            string id = GetUserIdAsync(username);
            string type_of_user_id = GetTypeOfUserAsync(username);


            var jwt_description = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {new Claim("username", username),
                                                    new Claim("id", id),
                                                    new Claim("type_of_user_id", type_of_user_id)
                                                   }),
                Expires = expiresLocalTime,
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = credentials
            };

            var token = new JwtSecurityTokenHandler().CreateToken(jwt_description);
            var encryptedToken = new JwtSecurityTokenHandler().WriteToken(token);

            _httpContextAccessor.HttpContext.Response.Cookies.Append("token", encryptedToken,
                new CookieOptions {
                    Expires = expiresLocalTime,
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });

            var response = new { token = encryptedToken, username = user.Username };
            return JsonSerializer.Serialize(response);
        }


        private string GetUserIdAsync(string username) {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user?.UserId.ToString();
        }

        private string GetTypeOfUserAsync(string username) {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user.TypeOfUserId.ToString();
        }

        /*
        public string DecodeToken() {
            var decodedToken1 = _httpContextAccessor.HttpContext.Request.Headers.Cookie;
            string decodedToken = decodedToken1.ToString();
            if (decodedToken == null) {
                // " the case where HttpContext is null
                return "ovo je cudno";
            } else {
                return decodedToken;
            }
        }
        */
    }
}
