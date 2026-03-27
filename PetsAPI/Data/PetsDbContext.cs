using Microsoft.EntityFrameworkCore;
using PetsAPI.Models;

namespace PetsAPI.Data
{
    public class PetsDbContext : DbContext
    {
        public PetsDbContext(DbContextOptions<PetsDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<HealthData> HealthData { get; set; }
        public DbSet<DailyLog> DailyLogs { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Pet relationship
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.User)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // 設定關聯
            modelBuilder.Entity<HealthData>()
                .HasOne(c => c.Pet)
                .WithMany(p => p.HealthData)
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
