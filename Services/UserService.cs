using ecommerce_API.Data;
using ecommerce_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Services
{
    public class UserService : IUserService
    {
        private readonly ecommerce_APIContext _context;
        public UserService(ecommerce_APIContext context)
        {
            _context = context;
        }
        public async Task<bool> VerifyUserPassword(User user)
        {
            bool verified = false;
            User? userFromDataBase = await _context.Users.Where(u => u.UserName == user.UserName).FirstOrDefaultAsync();
            if (userFromDataBase != null)
            {
                verified = BCrypt.Net.BCrypt.Verify(user.Password, userFromDataBase.Password);
                return verified;
            }
            else
            {
                return false;
            }
        }
    }
}
