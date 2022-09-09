using ecommerce_API.Entities.MainMenu;
using System.ComponentModel.DataAnnotations;

namespace ecommerce_API.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        [Required]
        public string title { get; set; } = string.Empty;
        [Required]
        public string address { get; set; } = string.Empty ;
        public ICollection<SubMenu> subMenus { get; set; } = new List<SubMenu>();
    }
}
