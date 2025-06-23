using Microsoft.EntityFrameworkCore;
using ImScoutAtWorker.Models;

public class RentalContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Server=localhost,1433;User Id=sa;Password=Z!uperPuperPassword123;Database=AustriaRentals;Encrypt=false")
                     .UseSnakeCaseNamingConvention()
                     .LogTo(Console.WriteLine);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flat>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.ImScoutId)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(b => b.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            entity.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
        });
    }
    public DbSet<Flat> Flats { get; set; }
}