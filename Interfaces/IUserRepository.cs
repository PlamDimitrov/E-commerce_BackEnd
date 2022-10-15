using ecommerce_API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_API.Interfaces
{
    public interface IUserRepository<T>
    {
        Task<UserDto?> Create(T userToCreate);
        Task<ICollection<UserDto>> GetAll();
        Task<T?> GetOne(int id);
        Task<T> GetOne(string username);
        Task<T>  Update(T user);
        Task<bool> Delete(int id);
        Task<UserDto?> LogIn(T userLogin);
        Task<UserDto> GetDto(int id);
        Task<UserDto?> RemoveImage(int id);
        Task<UserDto?> AddImage(int id, IFormFile file);
        bool CheckIfExists(int id);
        bool CheckIfExists(string emailOrUsername);
    }
}
