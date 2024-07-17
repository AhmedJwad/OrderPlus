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
    }
}
