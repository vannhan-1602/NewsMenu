using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<News> News => Set<News>();
        public DbSet<MenuNews> MenuNews => Set<MenuNews>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Ward> Wards => Set<Ward>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var fk in entityType.GetForeignKeys())
                {
                    fk.SetConstraintName(null);
                }
            }
        }
    }
}
