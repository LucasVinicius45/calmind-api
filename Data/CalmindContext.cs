using Calmind.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmind.Api.Data
{
    public class CalmindContext : DbContext
    {
        public CalmindContext(DbContextOptions<CalmindContext> options) : base(options)
        {
        }

        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<Capsule> Capsules { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collaborator>()
                .HasMany<Reservation>()
                .WithOne(r => r.Collaborator)
                .HasForeignKey(r => r.CollaboratorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Capsule>()
                .HasMany<Reservation>()
                .WithOne(r => r.Capsule)
                .HasForeignKey(r => r.CapsuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
