using Hair_saloon.Data;
using Hair_saloon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


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

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                new Claim[] {
                     new Claim("username", user.Username), 
                },
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto user) {
            IActionResult response = Unauthorized();
            var _user = await AuthenticateUser(user); 
            if (_user != null) {
                var token = GenerateJWT(_user);
                response = Ok(new { token = token });
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

