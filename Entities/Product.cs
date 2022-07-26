using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerce_API.Entities
{
    [Table("Product")]
    [Index("WebId", IsUnique = true)]
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string WebId { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        [Required]
        public string Condition { get; set; } = string.Empty;
        [Required]
        public bool Availability { get; set; } = false;
        [Required]
        public bool featuredItem { get; set; } = false;
        [Required]
        public bool recommended { get; set; } = false;

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
        public ICollection<CategoryProduct>? Categories { get; set; }
    }
}
