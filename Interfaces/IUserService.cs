namespace ecommerce_API.Interfaces
{
    public interface IUserService<T>
    {
        Task<bool> VerifyUserPassword(T user);
    }
}
