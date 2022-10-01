using ecommerce_API.Entities.MainMenu;
using System.ComponentModel.DataAnnotations;

namespace ecommerce_API.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty ;
        public ICollection<SubMenu> SubMenus { get; set; } = new List<SubMenu>();
    }
}
