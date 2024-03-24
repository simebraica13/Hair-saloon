using Hair_saloon.Data;
using Hair_saloon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace Hair_saloon.Controllers {
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {

        private IConfiguration _config;
        private readonly HairSaloonContext _context;
        public AuthController(IConfiguration configuration, HairSaloonContext context)
        {
            _config = configuration;
            _context = context;
        }
        private async Task<UserLoginDto> AuthenticateUser(UserLoginDto user) {
            UserLoginDto _user = null;
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null && BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password)) {
                _user = new UserLoginDto { Username = existingUser.Username };
            }
            return _user;
        }

        private async Task<User> CreateUser(User user) {
            User _user = null;
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            if (existingUser == null) {
                _user = new User {
                    TypeOfUserId = 1,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Password = passwordHash
                };
                _context.Users.Add(_user);
                await _context.SaveChangesAsync();
            }
            
            return _user;
        }

        private string GenerateJWT(UserLoginDto user) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var audience = _config["Jwt:Audience"];
            var issuer = _config["Jwt:Issuer"];
            TimeZoneInfo croatiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime expiresLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, croatiaTimeZone).AddMinutes(1);

            string username = user.Username;
            string id =  GetUserIdAsync(username);
            string type_of_user_id =  GetTypeOfUserAsync(username);


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

            HttpContext.Response.Cookies.Append("token", encryptedToken,
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
            var user =  _context.Users.FirstOrDefault(u => u.Username == username);
            return user?.UserId.ToString();
        }

        private string GetTypeOfUserAsync(string username) {
            var user =  _context.Users.FirstOrDefault(u => u.Username == username);
            return user.TypeOfUserId.ToString();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout() {
            HttpContext.Response.Cookies.Delete("token");
            var response = Ok();
            return response;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto user) {
            IActionResult response = Unauthorized();
            var _user = await AuthenticateUser(user); 
            if (_user != null) {
                var one = GenerateJWT(_user);
                response = Ok();
            }
            return response;
        }



        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user) {
            IActionResult response;
            var _user = await CreateUser(user);
            if (_user != null) {
                response = Ok();
            } else {
                response = BadRequest();
            }
            return response;
        }

    }
    
}

