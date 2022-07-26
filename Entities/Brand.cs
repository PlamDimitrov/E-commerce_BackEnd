﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerce_API.Entities
{
    [Table("Brands")]
    [Index("Name", IsUnique = true)]
    public class Brand
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public byte[]? Image { get; set; } = null;
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
