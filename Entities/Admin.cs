using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_API
{
        [Table("Admins")]
        [Index("userName", IsUnique = true)]
    public class Admin
    {

        public int Id { get; set; }
        [Required]
        public string userName { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = string.Empty;
        public byte[]? image { get; set; }
    }
}
