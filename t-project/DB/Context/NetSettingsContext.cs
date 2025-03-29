using Microsoft.EntityFrameworkCore;
using t_project.Database;
using t_project.Models;

namespace t_project.DB.Context
{
    public class NetSettingsContext : DbContext
    {
        public DbSet<NetSettings> NetSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NetSettings>()
                .HasOne(n => n.Equipment)
                .WithMany()
                .HasForeignKey(n => n.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade); // Опционально: каскадное удаление

            modelBuilder.Entity<NetSettings>()
                .HasIndex(n => n.IpAddress)
                .IsUnique();
        }
    }
}