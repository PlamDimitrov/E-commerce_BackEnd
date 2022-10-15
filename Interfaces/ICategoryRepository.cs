using ecommerce_API.Dto;
using ecommerce_API.Entities;

namespace ecommerce_API.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> Create(Category brand);
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetOne(int id);
        Task<Category> GetOne(string title);
        Task<Category> Update(Category brand);
        Task<bool> Delete(int id);
        Task<IEnumerable<Product>> GetCategoryProducts(int id);
        Task<IEnumerable<Product>> GetCategoryProducts(string title);
        Task<Category> RemoveImage(int id);
        Task<Category> AddImage(int id, IFormFile file);
        bool CheckIfExists(int id);
        bool CheckIfExists(string title);
    }
}
