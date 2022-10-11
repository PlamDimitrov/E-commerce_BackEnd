using ecommerce_API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_API.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        User GetUser(int id);
        User GetUser(string username);
        Task<User>  UpdateUser(User user);
        User CreateUser(User user);
        Task<UserDto?> LogInUser(User userLogin);
    }
}
