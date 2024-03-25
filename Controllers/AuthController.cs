using Hair_saloon.Data;
using Hair_saloon.Models;
using Hair_saloon.Services;
using Hair_saloon.Tokens;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Jwt _jwt;
        private LoginService _loginService;
        private RegistrationService _registrationService;

        public AuthController(IConfiguration configuration, HairSaloonContext context, IHttpContextAccessor httpContextAccessor) {
            _config = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jwt = new Jwt(_config, _httpContextAccessor, _context);
            _loginService = new LoginService(_context);
            _registrationService = new RegistrationService(_context);
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
            var _user = await _loginService.AuthenticateUser(user); 
            if (_user != null) {
                var token = _jwt.GenerateJWT(_user);
                response = Ok();
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user) {
            IActionResult response;
            var _user = await _registrationService.CreateUser(user);
            if (_user != null) {
                response = Ok();
            } else {
                response = BadRequest();
            }
            return response;
        }

    }
    
}

