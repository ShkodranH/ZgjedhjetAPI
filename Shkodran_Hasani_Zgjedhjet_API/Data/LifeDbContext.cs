using Microsoft.EntityFrameworkCore;
using Shkodran_Hasani_Zgjedhjet_API.Models.Entities;

namespace Shkodran_Hasani_Zgjedhjet_API.Data
{
    // YOUR CODE HERE
    public class LifeDbContext : DbContext
    {
        public LifeDbContext(DbContextOptions<LifeDbContext> options) : base(options)
        {
        }

        public DbSet<Zgjedhjet> Zgjedhjet { get; set; }
    }
}
