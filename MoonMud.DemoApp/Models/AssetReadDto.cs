namespace MoonMud.DemoApp.Models;

public class AssetReadDto
{
    public Guid GlobalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int Precision { get; set; }
    public DateTime CreatedAt { get; set; }
}