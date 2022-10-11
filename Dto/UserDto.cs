namespace ecommerce_API.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? Image { get; set; } = null;
    }
}
