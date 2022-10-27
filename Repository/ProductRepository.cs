using ecommerce_API.Data;
using ecommerce_API.Entities;
using ecommerce_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace ecommerce_API.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ecommerce_APIContext _context;
        public ProductRepository(ecommerce_APIContext context)
        {
            _context = context;
        }

        public async Task<Product?> Create(Product product)
        {
            try
            {
                _context.Products.Add(product);

                await _context.SaveChangesAsync();
                return await _context.Products.Where(p=> p.Title == product.Title).FirstAsync();
            }
            catch (Exception)
            {
                throw new Exception("Error: Creation of Product failed! Problem with database connection.");
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                Product? product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return false;
                }
                else
                {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Delete Product failed! Problem with database connection.");
            }
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        public Task<IEnumerable<Product>> GetBrandProducts(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetBrandProducts(string title)
        {
            throw new NotImplementedException();
        }

        public async Task<Product?> GetOne(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> GetOne(string name)
        {
            return await _context.Products.FindAsync(name);
        }

        public async Task<Product?> RemoveImage(int id)
        {
            Product? fromDataBase = await _context.Products
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
        public async Task<Product> AddImage(int id, IFormFile file)
        {
            Product? fromDataBase = await _context.Products
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

        public async Task<Product?> Update(Product product)
        {
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return await _context.Products.FindAsync(product.Id);
            }
            catch (Exception)
            {
                throw new Exception("Error: Update Product failed! Problem with database connection.");
            }
        }

        public bool CheckIfExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public bool CheckIfExists(string name)
        {
            return _context.Products.Any(e => e.Title == name);
        }
    }
}
