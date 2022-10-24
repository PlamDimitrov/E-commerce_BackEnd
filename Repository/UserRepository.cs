using ecommerce_API.Data;
using ecommerce_API.Dto;
using ecommerce_API.Helpers;
using ecommerce_API.Interfaces;
using ecommerce_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Repository
{
    public class UserRepository : IUserRepository<User>
    {
        private readonly ecommerce_APIContext _context;
        private readonly IUserService<User> _userService;
        public UserRepository(ecommerce_APIContext context, IUserService<User> userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<UserDto?> Create(User userToCreate)
        {
            User userWithHashedPassword = userToCreate;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userToCreate.Password);
            userWithHashedPassword.Password = passwordHash;
            try
            {
                _context.Users.Add(userWithHashedPassword);
                await _context.SaveChangesAsync();
                User? createdUser = await _context.Users
                    .Where(u => u.UserName == userToCreate.UserName)
                    .FirstOrDefaultAsync();
                if (createdUser != null)
                {
                    UserDto user = new UserDto();
                    user.Map(createdUser);
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Exception: " + sqlEx);
                return null;
                // throw new Exception("Error: Users not created!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                return null;
                // throw new Exception("Error: Users not created!");
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                User? user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw new Exception("Error: Something went wrong with the DB. Users not deleted!");
            }
        }
        public async Task<User?> GetOne(int id)
        {
            User? user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return user;
        }
        public async Task<User?> GetOne(string username)
        {
            User? user = await _context.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
            return user;
        }
        public async Task<UserDto?> GetDto(int id)
        {
            IUser? userFromDataBase = await _context.Users
                    .Where(u => u.Id == id)
                    .FirstOrDefaultAsync();
            if (userFromDataBase != null)
            {
                UserDto user = new UserDto();
                user.Map(userFromDataBase);
                return user;
            }
            else
            {
                return null;
            }
        }
        public async Task<ICollection<UserDto>> GetAll()
        {
            ICollection<User> users = await _context.Users.OrderBy(u => u.Id).ToListAsync();
            ICollection<UserDto> userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                UserDto userDto = new UserDto();
                userDto.Map(user);
                userDtos.Add(userDto);
            }
            return userDtos;
        }
        public async Task<User?> Update(User user)
        {
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
        public async Task<UserDto?> LogIn(User userLogin)
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
                    user.Map(userFromDataBase);
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
        public async Task<UserDto?> RemoveImage(int id)
        {
            User? userFromDataBase = await _context.Users
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (userFromDataBase == null)
            {
                return null;
            }
            else
            {
                userFromDataBase.Image = null;
                await _context.SaveChangesAsync();
                UserDto user = new UserDto();
                user.Map(userFromDataBase);
                return user;
            }
        }
        public async Task<UserDto?> AddImage(int id, IFormFile file)
        {
            User? userFromDataBase = await _context.Users
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (userFromDataBase == null)
            {
                return null;
            }
            else
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        byte[] fileBytes = ms.ToArray();

                        userFromDataBase.Image = fileBytes;
                        await _context.SaveChangesAsync();
                    };
                    UserDto user = new UserDto();
                    user.Map(userFromDataBase);
                    return user;
                }
                catch (Exception)
                {
                    throw new Exception("Error: Not found!");
                }
            }
        }
        public bool CheckIfExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        public bool CheckIfExists(string emailOrUsername)
        {
            return _context.Users.Any(e => e.Email == emailOrUsername || e.UserName == emailOrUsername);
        }
    }
}
