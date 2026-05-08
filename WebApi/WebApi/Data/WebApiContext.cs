using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class WebApiContext : DbContext
    {
        public WebApiContext(DbContextOptions<WebApiContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Donor> Donors => Set<Donor>();
        public DbSet<Gift> Gifts => Set<Gift>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Basket> Baskets => Set<Basket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(10);
                entity.Property(e => e.City).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Street).IsRequired().HasMaxLength(30);
                entity.Property(e => e.BuildingNumber).IsRequired();
            });
            modelBuilder.Entity<Donor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(30);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Phone).HasMaxLength(10);
            });
            modelBuilder.Entity<Gift>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(g => g.PriceCard)
        .HasColumnType("decimal(18,2)"); entity.Property(e => e.Quantity).IsRequired().HasMaxLength(10);
                entity.Property(e=>e.PictureId).IsRequired();

            });
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id);

            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();    
            });

        }


    }
}
