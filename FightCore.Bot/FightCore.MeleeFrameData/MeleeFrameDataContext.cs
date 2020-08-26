using Microsoft.EntityFrameworkCore;

namespace FightCore.MeleeFrameData
{
    public class MeleeFrameDataContext : DbContext
    {
        public DbSet<Attack> Attacks { get; set; }

        public DbSet<Dodge> Dodges { get; set; }

        public DbSet<Grab> Grabs { get; set; }

        public DbSet<Throw> Throws { get; set; }

        public DbSet<Misc> Misc { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=Data/characters.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attack>().HasNoKey();
            modelBuilder.Entity<Attack>().HasNoKey();
            modelBuilder.Entity<Dodge>().HasNoKey();
            modelBuilder.Entity<Grab>().HasNoKey();
            modelBuilder.Entity<Throw>().HasNoKey();
            modelBuilder.Entity<Misc>().HasNoKey();
        }
    }
}
