using ecommerce_API.Entities;

namespace ecommerce_API.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> Create(Product brand);
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetOne(int id);
        Task<Product> GetOne(string title);
        Task<Product> Update(Product brand);
        Task<bool> Delete(int id);
        Task<Product> RemoveImage(int id);
        Task<Product> AddImage(int id, IFormFile file);
        bool CheckIfExists(int id);
        bool CheckIfExists(string title);
    }
}
