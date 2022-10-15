using ecommerce_API.Data;
using ecommerce_API.Entities;
using ecommerce_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_API.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ecommerce_APIContext _context;
        public CategoryRepository(ecommerce_APIContext context)
        {
            _context = context;
        }

        public async Task<Category?> Create(Category Category)
        {
            try
            {
                _context.Categories.Add(Category);
                await _context.SaveChangesAsync();
                return await _context.Categories.FindAsync(Category.Name);
            }
            catch (Exception)
            {
                throw new Exception("Error: Creation of Category failed! Problem with database connection.");
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                Category? Category = await _context.Categories.FindAsync(id);
                if (Category == null)
                {
                    return false;
                }
                else
                {
                    _context.Categories.Remove(Category);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Delete Category failed! Problem with database connection.");
            }
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }

        public Task<IEnumerable<Product>> GetCategoryProducts(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetCategoryProducts(string title)
        {
            throw new NotImplementedException();
        }

        public async Task<Category?> GetOne(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category?> GetOne(string name)
        {
            return await _context.Categories.FindAsync(name);
        }

        public async Task<Category?> RemoveImage(int id)
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
        public async Task<Category> AddImage(int id, IFormFile file)
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

        public async Task<Category?> Update(Category Category)
        {
            try
            {
                _context.Categories.Update(Category);
                await _context.SaveChangesAsync();
                return await _context.Categories.FindAsync(Category.Id);
            }
            catch (Exception)
            {
                throw new Exception("Error: Update Category failed! Problem with database connection.");
            }
        }

        public bool CheckIfExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        public bool CheckIfExists(string name)
        {
            return _context.Categories.Any(e => e.Name == name);
        }
    }
}
