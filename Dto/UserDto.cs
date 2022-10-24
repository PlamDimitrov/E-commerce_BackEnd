using ecommerce_API.Interfaces;

namespace ecommerce_API.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? Image { get; set; } = null;

        public void Map(IUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            Image = user.Image;
        }
    }
}
