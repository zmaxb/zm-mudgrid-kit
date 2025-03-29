namespace MoonGridBuilder.Core;

public class DataGridOptions
{
    public string Title { get; set; } = string.Empty;
    public int[] PageSizeOptions { get; set; } = [10, 25, 50, 100];
}