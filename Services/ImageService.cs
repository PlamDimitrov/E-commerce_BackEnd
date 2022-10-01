using ecommerce_API.Data;
using ecommerce_API.Entities;
using ecommerce_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Services
{
    public class ImageService
    {
        private readonly ecommerce_APIContext _context;

        public ImageService(ecommerce_APIContext context)
        {
            _context = context;
        }

        public async Task<Brand?> RemoveFromBrand(int id)
        {
            Brand? fromDataBase = await _context.Brands
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (fromDataBase == null)
            {
                return null;
            }
            else
            {
                fromDataBase.Image = null;
                await _context.SaveChangesAsync();
                return fromDataBase;
            }
        }
        public async Task<Category?> RemoveFromCategory(int id)
        {
            Category? fromDataBase = await _context.Categories
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (fromDataBase == null)
            {
                return null;
            }
            else
            {
                fromDataBase.Image = null;
                await _context.SaveChangesAsync();
                return fromDataBase;
            }
        }
        public async Task<IUser?> RemoveFromUser(int id)
        {
            User? fromDataBase = await _context.Users
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (fromDataBase == null)
            {
                return null;
            }
            else
            {
                fromDataBase.Image = null;
                await _context.SaveChangesAsync();
                return fromDataBase;
            }
        }
        public async Task<IUser?> RemoveFromAdmin(int id)
        {
            Admin? fromDataBase = await _context.Admins
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (fromDataBase == null)
            {
                return null;
            }
            else
            {
                fromDataBase.Image = null;
                await _context.SaveChangesAsync();
                return fromDataBase;
            }
        }
        public async Task<Brand?> AddToBrand(int id, IFormFile file)
        {
            Brand? fromDataBase = await _context.Brands
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (fromDataBase == null)
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

                        fromDataBase.Image = fileBytes;
                        await _context.SaveChangesAsync();
                    };
                    return fromDataBase;
                }
                catch (Exception)
                {
                    throw new Exception("Error: Not found!");
                }
            }
        }
        public async Task<Category?> AddToCategory(int id, IFormFile file)
        {
            Category? fromDataBase = await _context.Categories
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (fromDataBase == null)
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

                        fromDataBase.Image = fileBytes;
                        await _context.SaveChangesAsync();
                    };
                    return fromDataBase;
                }
                catch (Exception)
                {
                    throw new Exception("Error: Not found!");
                }
            }
        }
        public async Task<IUser?> AddToUser(int id, IFormFile file)
        {
            User? fromDataBase = await _context.Users
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (fromDataBase == null)
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

                        fromDataBase.Image = fileBytes;
                        await _context.SaveChangesAsync();
                    };
                    return fromDataBase;
                }
                catch (Exception)
                {
                    throw new Exception("Error: Not found!");
                }
            }
        }
        public async Task<IUser?> AddToAdmin(int id, IFormFile file)
        {
            Admin? fromDataBase = await _context.Admins
                        .Where(e => e.Id == id)
                        .FirstOrDefaultAsync();
            if (fromDataBase == null)
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

                        fromDataBase.Image = fileBytes;
                        await _context.SaveChangesAsync();
                    };
                    return fromDataBase;
                }
                catch (Exception)
                {
                    throw new Exception("Error: Not found!");
                }
            }
        }
    }
}
