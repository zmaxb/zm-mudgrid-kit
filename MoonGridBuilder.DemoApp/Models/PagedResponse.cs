using System.Collections.Generic;

namespace MoonMud.DemoApp.Models;

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}
