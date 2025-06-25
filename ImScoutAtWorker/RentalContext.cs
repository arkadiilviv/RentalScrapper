using Microsoft.EntityFrameworkCore;
using ImScoutAtWorker.Models;

public class RentalContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer(SystemInfo.SQL_CONNECTION_STRING)
                     .UseSnakeCaseNamingConvention()
                     .LogTo(Console.WriteLine);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flat>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.HasAlternateKey(b => b.Hash);

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