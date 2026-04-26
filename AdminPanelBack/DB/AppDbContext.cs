using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.DB;
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<Admin>(options)
{
    public  DbSet<User> Clients { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<BroadcastMessage> BroadcastMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<Feedback>()
            .HasKey(f => f.Id);

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany(u => u.Feedbacks)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(t => t.TokenHash)
            .IsUnique()
            // Allow old rows that haven't been backfilled yet during rollout.
            .HasFilter("\"TokenHash\" <> ''");
    }
}
