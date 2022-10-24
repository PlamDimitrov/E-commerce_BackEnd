using ecommerce_API.Data;
using ecommerce_API.Dto;
using ecommerce_API.Interfaces;
using ecommerce_API.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Repository
{
    public class AdminRepository : IUserRepository<Admin>
    {
        private readonly ecommerce_APIContext _context;
        private readonly IUserService<Admin> _adminService;
        public AdminRepository(ecommerce_APIContext context, IUserService<Admin> adminService)
        {
            _context = context;
            _adminService = adminService;
        }
        public async Task<UserDto?> Create(Admin adminToCreate)
        {
            Admin adminWithHashedPassword = adminToCreate;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(adminToCreate.Password);
            adminWithHashedPassword.Password = passwordHash;
            try
            {
                _context.Admins.Add(adminWithHashedPassword);
                await _context.SaveChangesAsync();
                Admin? createdAdmin = await _context.Admins
                    .Where(u => u.UserName == adminToCreate.UserName)
                    .FirstOrDefaultAsync();
                if (createdAdmin != null)
                {
                    UserDto admin = new UserDto();
                    admin.Map(createdAdmin);
                    return admin;
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
                // throw new Exception("Error: Admins not created!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                return null;
                // throw new Exception("Error: Admins not created!");
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                Admin? Admin = await _context.Admins.FindAsync(id);
                if (Admin != null)
                {
                    _context.Admins.Remove(Admin);
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
                throw new Exception("Error: Something went wrong with the DB. Admins not deleted!");
            }
        }
        public async Task<Admin?> GetOne(int id)
        {
            Admin? Admin = await _context.Admins.Where(u => u.Id == id).FirstOrDefaultAsync();
            return Admin;
        }
        public async Task<Admin> GetOne(string username)
        {
            Admin Admin = await _context.Admins.Where(u => u.UserName == username).FirstOrDefaultAsync();
            return Admin;
        }
        public async Task<UserDto?> GetDto(int id)
        {
            IUser? adminFromDataBase = await _context.Admins
                    .Where(u => u.Id == id)
                    .FirstOrDefaultAsync();
            if (adminFromDataBase != null)
            {
                UserDto admin = new UserDto();
                admin.Map(adminFromDataBase);
                return admin;
            }
            else
            {
                return null;
            }
        }
        public async Task<ICollection<UserDto>> GetAll()
        {
            ICollection<Admin> admins = await _context.Admins.OrderBy(u => u.Id).ToListAsync();
            ICollection<UserDto> adminDtos = new List<UserDto>();
            foreach (var admin in admins)
            {
                UserDto adminDto = new UserDto();
                adminDto.Map(admin);
                adminDtos.Add(adminDto);
            }
            return adminDtos;
        }
        public async Task<Admin> Update(Admin Admin)
        {
            _context.Entry(Admin).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                Admin? updatedAdmin = await _context.Admins.Where(u => u.Id == Admin.Id).FirstOrDefaultAsync();
                return updatedAdmin;
            }
            catch (Exception)
            {
                throw new Exception("Error: Update admin failed! Problem with database connection.");
            }
        }
        public async Task<UserDto?> LogIn(Admin adminLogin)
        {
            try
            {
                Admin? adminFromDataBase = await _context.Admins
                .Where(u => u.UserName == adminLogin.UserName)
                    .FirstOrDefaultAsync();
                bool verified = await _adminService.VerifyUserPassword(adminLogin);
                if (verified && adminFromDataBase != null)
                {
                    UserDto admin = new UserDto();
                    admin.Map(adminFromDataBase);
                    return admin;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Something went wrong with the database on admin Login!");
            }
        }
        public async Task<UserDto?> RemoveImage(int id)
        {
            Admin? adminFromDataBase = await _context.Admins
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (adminFromDataBase == null)
            {
                return null;
            }
            else
            {
                adminFromDataBase.Image = null;
                await _context.SaveChangesAsync();
                UserDto admin = new UserDto();
                admin.Map(adminFromDataBase);
                return admin;
            }
        }
        public async Task<UserDto?> AddImage(int id, IFormFile file)
        {
            Admin? adminFromDataBase = await _context.Admins
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (adminFromDataBase == null)
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

                        adminFromDataBase.Image = fileBytes;
                        await _context.SaveChangesAsync();
                    };
                    UserDto admin = new UserDto();
                    admin.Map(adminFromDataBase);
                    return admin;
                }
                catch (Exception)
                {
                    throw new Exception("Error: Not found!");
                }
            }
        }
        public bool CheckIfExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
        public bool CheckIfExists(string emailOrUsername)
        {
            return _context.Admins.Any(e => e.Email == emailOrUsername || e.UserName == emailOrUsername);
        }
    }
}
