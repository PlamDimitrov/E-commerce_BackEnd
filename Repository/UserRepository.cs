using ecommerce_API.Data;
using ecommerce_API.Dto;
using ecommerce_API.Helpers;
using ecommerce_API.Interfaces;
using ecommerce_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ecommerce_APIContext _context;
        private readonly IUserService _userService;
        public UserRepository(ecommerce_APIContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
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
        public async Task<UserDto?> LogInUser(User userLogin)
        {
            try
            {
                User? userFromDataBase = await _context.Users
                    .Where(u => u.UserName == userLogin.UserName)
                    .FirstOrDefaultAsync();
                bool verified = await _userService.VerifyUserPassword(userLogin);
                if (verified && userFromDataBase != null)
                {
                    UserDto user = new UserDto();
                    user.Id = userFromDataBase.Id;
                    user.UserName = userFromDataBase.UserName;
                    user.Email = userFromDataBase.Email;
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Something went wrong with the database on User Login!");
            }
        }
    }
}
