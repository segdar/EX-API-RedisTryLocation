using EX_API_RedisTryLocation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace EX_API_RedisTryLocation.Services
{
    public class ContextDBServices : DbContext
    {
        public ContextDBServices(
           DbContextOptions<ContextDBServices> options)
           : base(options)
        {
        }

        public DbSet<NearLocation> locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NearLocation>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null);
            });
        }
    }
}
