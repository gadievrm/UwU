using Microsoft.EntityFrameworkCore;
using t_project.Database;
using t_project.Models;

namespace t_project.DB.Context
{
    public class ModelTypeContext : DbContext
    {
        public DbSet<ModelType> ModelTypes { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        public ModelTypeContext()
        {
            Database.EnsureCreated();
            ModelTypes.Load();
            EquipmentTypes.Load();
            Equipments.Load();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelType>()
                .HasOne(m => m.Type)
                .WithMany()
                .HasForeignKey(m => m.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.ModelType)
                .WithMany()
                .HasForeignKey(e => e.ModelTypeId)
                .OnDelete(DeleteBehavior.SetNull); // Устанавливаем null при удалении типа
        }
    }
}