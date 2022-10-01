using System.Text.Json.Serialization;

namespace ecommerce_API.Entities.MainMenu
{
    public class SubMenuLinks
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int SubMenuId { get; set; }
        [JsonIgnore]
        public SubMenu SubMenu { get; set; } = new SubMenu();
    }
}
