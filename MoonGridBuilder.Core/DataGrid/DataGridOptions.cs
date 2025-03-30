namespace MoonGridBuilder.Core.DataGrid;

public class DataGridOptions
{
    public string Title { get; set; } = string.Empty;
    public bool MultiSelection { get; set; } = true;
    public bool SearchEnabled { get; set; } = true;
    public bool ShowMenuIcon { get; set; } = false;
    public bool ShowColumnOptions { get; set; } = false;
    public bool ShowFilterIcons { get; set; } = false;
    public bool ShowPager { get; set; } = true;
    public int DefaultPageSize { get; set; } = 10;
    public int[] PageSizeOptions { get; set; } = [10, 25, 50, 100];
}