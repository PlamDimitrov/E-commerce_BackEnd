#nullable disable
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Entities;
using ecommerce_API.Entities.MainMenu;

namespace ecommerce_API.Data
{
    public class ecommerce_APIContext : DbContext
    {
        public ecommerce_APIContext (DbContextOptions<ecommerce_APIContext> options)
            : base(options)
        {
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ExpiredToken> ExpiredTokens { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<SubMenu> SubMenu { get; set; }
        public DbSet<SubMenuLinks> SubMenuLinks { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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

            builder.Entity<Menu>()
                .HasMany(m => m.SubMenus)
                .WithOne(s => s.Menu);
            builder.Entity<SubMenu>()
                .HasMany(m => m.Links)
                .WithOne(s => s.SubMenu);
        }
    }
}
