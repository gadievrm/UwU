using Microsoft.EntityFrameworkCore;
using t_project.Database;
using t_project.Models;

namespace t_project.DB.Context
{
    public class ProgrammsContext : DbContext
    {
        public DbSet<Programms> Programms { get; set; }

        public ProgrammsContext()
        {
            Database.EnsureCreated();
            Programms.Load();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}