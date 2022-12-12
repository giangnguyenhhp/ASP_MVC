using Microsoft.EntityFrameworkCore;

namespace ASP_MVC.Models;

public class MasterDbContext : DbContext
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
        base.OnModelCreating(modelbuilder);
    }

}