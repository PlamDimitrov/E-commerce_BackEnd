namespace ecommerce_API.Interfaces
{
    public interface IUser
    {
        string Email { get; set; }
        int Id { get; set; }
        byte[]? Image { get; set; }
        string Password { get; set; }
        string UserName { get; set; }
    }
}