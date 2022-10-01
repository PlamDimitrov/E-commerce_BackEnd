using ecommerce_API.Data;
using ecommerce_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Services
{
    public class PictureService 
    {
        private readonly ecommerce_APIContext _context;

        public PictureService(ecommerce_APIContext context)
        {
            _context = context;
        }

        public async Task<Brand?> RemoveFromBrand(int id)
        {
            Brand? brandFromDataBase = await _context.Brand
                        .Where(u => u.Id == id)
                        .FirstOrDefaultAsync();
            if (brandFromDataBase == null)
            {
                return null;
            }
            else
            {
                brandFromDataBase.Image = null;
                await _context.SaveChangesAsync();
                return brandFromDataBase;
            }
        }
        public async Task<Brand?> AddToBrand(int id, IFormFile file)
        {
            Brand? brandFromDataBase = await _context.Brand
                        .Where(u => u.Id == id)
                        .FirstOrDefaultAsync();
            if (brandFromDataBase == null)
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

                        brandFromDataBase.Image = fileBytes;
                        await _context.SaveChangesAsync();
                        Brand? brandFromDataBaseResponse = await _context.Brand
                                .Where(u => u.Id == id)
                                .FirstOrDefaultAsync();
                    };
                    return brandFromDataBase;
                }
                catch (Exception)
                {
                    throw new Exception("Error: Category not found!");
                }
            }
        }
    }
}
