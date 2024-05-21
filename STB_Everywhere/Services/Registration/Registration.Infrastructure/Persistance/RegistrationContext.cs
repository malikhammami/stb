using Microsoft.EntityFrameworkCore;
using Registration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Infrastructure.Persistance
{
    public class RegistrationContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        

        public RegistrationContext(DbContextOptions<RegistrationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntities(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Client>().HasKey(c => c.Id);
            modelBuilder.Entity<Client>().OwnsOne(c => c.Address);

            

        }
    }
}
