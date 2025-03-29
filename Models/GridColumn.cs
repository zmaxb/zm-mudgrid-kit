namespace MoonMud.DemoApp.Models;

using System;
using System.Linq.Expressions;

public class GridColumn<T>
{
    public Expression<Func<T, object>> Property { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
}