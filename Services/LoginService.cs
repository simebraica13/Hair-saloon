using Hair_saloon.Data;
using Hair_saloon.Models;
using Microsoft.EntityFrameworkCore;

namespace Hair_saloon.Services {
    public class LoginService {

        private HairSaloonContext _context;
        public LoginService(HairSaloonContext context)
        {
            _context = context;
        }
        public async Task<UserLoginDto> AuthenticateUser(UserLoginDto user) {
            UserLoginDto _user = null;
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null && BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password)) {
                _user = new UserLoginDto { Username = existingUser.Username };
            }
            return _user;
        }
    }
}
