using ecommerce_API.Data;
using ecommerce_API.Entities;
using ecommerce_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace ecommerce_API.Repository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ecommerce_APIContext _context;
        public BrandRepository(ecommerce_APIContext context)
        {
            _context = context;
        }

        public async Task<Brand?> Create(Brand brand)
        {
            try
            {
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                return await _context.Brands.FindAsync(brand.Name);
            }
            catch (Exception)
            {
                throw new Exception("Error: Creation of Brand failed! Problem with database connection.");
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                Brand? brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    return false;
                }
                else
                {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                return true;
                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Delete Brand failed! Problem with database connection.");
            }
        }

        public async Task<IEnumerable<Brand>> GetAll()
        {
            return await _context.Brands.ToListAsync();
        }

        public Task<IEnumerable<Product>> GetBrandProducts(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetBrandProducts(string title)
        {
            throw new NotImplementedException();
        }

        public async Task<Brand?> GetOne(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<Brand?> GetOne(string name)
        {
            return await _context.Brands.FindAsync(name);
        }

        public async Task<Brand?> RemoveImage(int id)
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
        public async Task<Brand> AddImage(int id, IFormFile file)
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

        public async Task<Brand?> Update(Brand brand)
        {
            try
            {
                _context.Brands.Update(brand);
                await _context.SaveChangesAsync();
                return await _context.Brands.FindAsync(brand.Id);
            }
            catch (Exception)
            {
                throw new Exception("Error: Update Brand failed! Problem with database connection.");
            }
        }

        public bool CheckIfExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }

        public bool CheckIfExists(string name)
        {
            return _context.Brands.Any(e => e.Name == name);
        }
    }
}
