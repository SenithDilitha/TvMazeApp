using System.Text.Json.Serialization;

namespace TvMazeApp.Domain.Entities;

public class Show
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Language { get; set; } = string.Empty;
    public DateTime? Premiered { get; set; }
    public string? Summary { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}