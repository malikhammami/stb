using Agence.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Agence.API.Persistance
{
    public class AgenceDbContext : DbContext
    {
        public DbSet<Models.Agence> Agences { get; set; }

        public AgenceDbContext(DbContextOptions<AgenceDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntities(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
        }
    }
}
