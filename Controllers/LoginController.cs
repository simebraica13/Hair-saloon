using Hair_saloon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hair_saloon.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase {
        private IConfiguration _config;
        public LoginController(IConfiguration configuration)
        {
            _config = configuration;
        }
        private User AuthenticateUser(User user) {
            User _user = null;
            if (user.Username == "Sime" && user.Password == "braica") {
                _user = new User { Username = "Wat" };
            }
                return _user;
        }
        private string GenerateJWT(User user) {
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
        [HttpPost]
        public IActionResult Login(User user) {
            IActionResult response = Unauthorized();
            var _user = AuthenticateUser(user);
            if (_user != null) {
                var token = GenerateJWT(_user);
                response = Ok(new { token = token });
            }
            return response;
        }
        }
    }

