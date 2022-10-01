using ecommerce_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_API
{
        [Table("Admins")]
        [Index("UserName", IsUnique = true)]
    public class Admin : IUser
    {

        public int Id { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? Image { get; set; } = null;
    }
}
