using ASP_MVC.Models.Blog;
using ASP_MVC.Models.Contacts;
using ASP_MVC.Models.Product;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Models;

public class MasterDbContext : IdentityDbContext<AppUser>
{
    
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        modelbuilder.ApplyUtcDateTimeConverter();//Put before seed data and after model creation
        base.OnModelCreating(modelbuilder);
        foreach (var entityType in modelbuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }

        modelbuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Slug).IsUnique();
        });

        modelbuilder.Entity<Post>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique();
        });
        
        modelbuilder.Entity<ProductModel>(entity =>
        {
            entity.HasIndex(c => c.Slug).IsUnique();
        });

        modelbuilder.Entity<CategoryProduct>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique();
        });
    }
    
    public DbSet<Contact> Contacts { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Post> Posts { get; set; }
    
    
    public DbSet<ProductModel> Products { get; set; }
    public DbSet<CategoryProduct> CategoryProducts { get; set; }
    public DbSet<ProductPhoto> ProductPhotos { get; set; }
}