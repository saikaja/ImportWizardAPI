using Microsoft.EntityFrameworkCore;
using ImportWizard.Data.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Alliance> Alliances { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategorySection> CategorySections { get; set; }
    public DbSet<SectionColumn> SectionColumns { get; set; }
}
