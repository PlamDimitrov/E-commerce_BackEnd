using ecommerce_API.Data;
using ecommerce_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Services
{
    public class AdmionService : IUserService<Admin>
    {
        private readonly ecommerce_APIContext _context;
        public AdmionService(ecommerce_APIContext context)
        {
            _context = context;
        }
        public async Task<bool> VerifyUserPassword(Admin user)
        {
            bool verified = false;
            Admin? userFromDataBase = await _context.Admins.Where(u => u.UserName == user.UserName).FirstOrDefaultAsync();
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
