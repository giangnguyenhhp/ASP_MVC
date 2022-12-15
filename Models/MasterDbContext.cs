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
    }

}