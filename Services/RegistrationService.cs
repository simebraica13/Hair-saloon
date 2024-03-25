using Hair_saloon.Data;
using Hair_saloon.Models;
using Microsoft.EntityFrameworkCore;

namespace Hair_saloon.Services {
    public class RegistrationService {

        private HairSaloonContext _context;
        public RegistrationService(HairSaloonContext context) {
            _context = context;
        }
        public async Task<User> CreateUser(User user) {
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
    }
}
