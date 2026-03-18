using Microsoft.EntityFrameworkCore;
using PetsAPI.Models;

namespace PetsAPI.Data
{
    public class PetsDbContext : DbContext
    {
        public PetsDbContext(DbContextOptions<PetsDbContext> options) : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<CollarData> CollarData { get; set; }
        public DbSet<DailyLog> DailyLogs { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設定關聯
            modelBuilder.Entity<CollarData>()
                .HasOne(c => c.Pet)
                .WithMany(p => p.CollarData)
                .HasForeignKey(c => c.PetID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DailyLog>()
                .HasOne(d => d.Pet)
                .WithMany(p => p.DailyLogs)
                .HasForeignKey(d => d.PetID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PetID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.Alerts)
                .HasForeignKey(a => a.PetID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
