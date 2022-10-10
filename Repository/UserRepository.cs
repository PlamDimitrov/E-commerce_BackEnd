using ecommerce_API.Data;
using ecommerce_API.Interfaces;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ecommerce_APIContext _context;
        public UserRepository(ecommerce_APIContext context)
        {
            _context = context;
        }

        public User CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        public User GetUser(int id)
        {
            User user = _context.Users.Where(u => u.Id == id).FirstOrDefault();
            return user;
        }

        public User GetUser(string username)
        {
            User user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();
            return user;
        }

        public ICollection<User> GetUsers()
        {
            ICollection<User> users = _context.Users.OrderBy(u => u.Id).ToList();
            return users;
        }

        public async Task<User?> UpdateUser(User user)
        {
            var promise = new TaskCompletionSource<User>();
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                User? updatedUser = await _context.Users.Where(u => u.Id == user.Id).FirstOrDefaultAsync();
                return updatedUser;
            }
            catch (Exception)
            {
                throw new Exception("Error: Update User failed! Problem with database connection.");
            }
        }
    }
}
