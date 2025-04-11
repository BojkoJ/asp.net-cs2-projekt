using BOJ0043_Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BOJ0043_Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CoworkingSpace> CoworkingSpaces { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceStatusHistory> WorkspaceStatusHistory { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurace pro CoworkingSpace
            modelBuilder.Entity<CoworkingSpace>()
                .HasMany(c => c.Workspaces)
                .WithOne(w => w.CoworkingSpace)
                .HasForeignKey(w => w.CoworkingSpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Konfigurace pro Workspace
            modelBuilder.Entity<Workspace>()
                .HasMany(w => w.StatusHistory)
                .WithOne(h => h.Workspace)
                .HasForeignKey(h => h.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Workspace>()
                .HasMany(w => w.Reservations)
                .WithOne(r => r.Workspace)
                .HasForeignKey(r => r.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Konfigurace pro Reservation
            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalPrice)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
