using System.Text.Json.Serialization;

namespace ecommerce_API.Entities.MainMenu
{
    public class SubMenuLinks
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int subMenuId { get; set; }
        [JsonIgnore]
        public SubMenu subMenu { get; set; } = new SubMenu();
    }
}
