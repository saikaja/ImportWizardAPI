using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImportWizard.Data.Models;

namespace ImportWizard.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategorySection> CategorySections { get; set; }
        public DbSet<SectionColumn> SectionColumns { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<ImportMaster> ImportMasters { get; set; }
    }
}
