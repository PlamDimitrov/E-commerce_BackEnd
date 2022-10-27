
using System.Text.Json.Serialization;

namespace ecommerce_API.Entities
{
    public class CategoryProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
    }
}
