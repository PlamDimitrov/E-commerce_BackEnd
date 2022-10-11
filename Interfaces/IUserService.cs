namespace ecommerce_API.Interfaces
{
    public interface IUserService
    {
        Task<bool> VerifyUserPassword(User user);
    }
}
