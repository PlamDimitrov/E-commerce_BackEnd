using ecommerce_API.Dto;
using ecommerce_API.Entities;

namespace ecommerce_API.Interfaces
{
    public interface IBrandRepository
    {
        Task<Brand> Create(Brand brand);
        Task<IEnumerable<Brand>> GetAll();
        Task<Brand> GetOne(int id);
        Task<Brand> GetOne(string title);
        Task<Brand> Update(Brand brand);
        Task<bool> Delete(int id);
        Task<IEnumerable<Product>> GetBrandProducts(int id);
        Task<IEnumerable<Product>> GetBrandProducts(string title);
        Task<Brand> RemoveImage(int id);
        Task<Brand> AddImage(int id, IFormFile file);
        bool CheckIfExists(int id);
        bool CheckIfExists(string title);
    }
}
