using Microsoft.EntityFrameworkCore;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Models;

namespace VehicleRegistry.InfraStructure.Database
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<VehicleDTO> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VehicleDTO>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Model).IsRequired();
                entity.Property(e => e.Year).IsRequired();
                entity.Property(e => e.Plate).IsRequired();
            });
        }
    }
}