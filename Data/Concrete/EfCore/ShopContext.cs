using Data.Configuration;
using Entity;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete.EfCore
{
    public class ShopContext : DbContext
    {
        public ShopContext()
        {
        }
        public ShopContext(DbContextOptions<ShopContext> contextOptions) : base(contextOptions)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Courser> Courser{get; set;}
        public DbSet<Image> Images{get;set;}
        public DbSet<Cart> Carts{get;set;}
        public DbSet<CartItem> CartItems{get;set;}
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<FavoriteProduct> FavoriteProduct { get; set; }
        public DbSet<ProductComment> Comments{get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder.ApplyConfiguration(new ProductConfiguration());
             modelBuilder.ApplyConfiguration(new CategoryConfiguration());
             modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
             modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
             modelBuilder.ApplyConfiguration(new ImageConfiguration());
             modelBuilder.ApplyConfiguration(new CourserConfiguration());
             modelBuilder.ApplyConfiguration(new FavoriteProductConfiguration());
             modelBuilder.ApplyConfiguration(new RatingConfiguration());
             modelBuilder.ApplyConfiguration(new CommentConfiguration());
             modelBuilder.Seed();
        }
           protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=.\\SQLEXPRESS;database=KokoMijaDb;integrated security=SSPI;Encrypt=False;");
        }

    }
}