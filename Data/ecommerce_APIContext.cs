#nullable disable
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Entities;

namespace ecommerce_API.Data
{
    public class ecommerce_APIContext : DbContext
    {
        public ecommerce_APIContext (DbContextOptions<ecommerce_APIContext> options)
            : base(options)
        {
        }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<ExpiredToken> ExpiredTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CategoryProduct>()
                .HasKey(x => new { x.CategoryId, x.ProductId });
            builder.Entity<CategoryProduct>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId);
            builder.Entity<CategoryProduct>()
                .HasOne(x => x.Product)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
