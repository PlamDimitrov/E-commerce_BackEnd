using System.Text.Json.Serialization;

namespace ecommerce_API.Entities.MainMenu
{
    public class SubMenu
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public int MenuId { get; set; }
        [JsonIgnore]
        public Menu? Menu { get; set; }
        public ICollection<SubMenuLinks> Links { get; set; } = new List<SubMenuLinks>();

    }
}
