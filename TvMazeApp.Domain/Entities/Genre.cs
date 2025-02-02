namespace TvMazeApp.Domain.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Show> Shows { get; set; } = new List<Show>();
}