namespace MoonMud.DemoApp.Models;

public class AssetCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int Precision { get; set; }
}