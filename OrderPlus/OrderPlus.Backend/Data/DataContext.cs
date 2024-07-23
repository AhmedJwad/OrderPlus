using Microsoft.EntityFrameworkCore;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext>options):base(options)
        {
                
        }
        public DbSet<Country>Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(x=>x.Name).IsUnique();
            modelBuilder.Entity<State>().HasIndex(x=> new {x.CountryId, x.Name}).IsUnique();
            modelBuilder.Entity<City>().HasIndex(x=> new { x.StateId, x.Name }).IsUnique();
            DisableCascadingDelete(modelBuilder);
        }

        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationship = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var item in relationship)
            {
                item.DeleteBehavior = DeleteBehavior.Restrict;
            }


        }
    }
}
