using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerce_API
{
    [Table("Users")]
    [Index("email", IsUnique = true)]
    [Index("userName", IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string userName { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = string.Empty;
        [Required]
        public string email { get; set; } = string.Empty;
        public string imageUrl { get; set; } = string.Empty;
        public bool isAdmin { get; set; } = false;
    }
}
