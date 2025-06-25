
namespace ImScoutAtWorker.Models;

public class Flat
{
    public int Id { get; set; }
    public string Hash { get; set; } = null!; // Unique identifier for the flat
    public string? ImScoutId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public double? Rooms { get; set; }
    public double? Area { get; set; }
    public string? City { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
