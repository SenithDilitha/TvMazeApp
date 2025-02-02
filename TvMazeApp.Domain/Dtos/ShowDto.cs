namespace TvMazeApp.Domain.Dtos;

public class ShowDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public DateTime? Premiered { get; set; }
    public string Summary { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new List<string>();
}