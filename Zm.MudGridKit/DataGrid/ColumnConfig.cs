using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Zm.MudGridKit.DataGrid;

public class ColumnConfig<T>
{
    public bool IsSelectionColumn { get; set; } = false;
    public LambdaExpression Property { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public bool Sortable { get; set; } = true;
    public bool Filterable { get; set; } = true;

    public string? CellStyle { get; set; }
    public string? HeaderStyle { get; set; }
    public string? Format { get; set; }
    public bool Visible { get; set; } = true;
    public Type PropertyType { get; set; } = null!;
    public RenderFragment<CellContext<T>>? Template { get; set; }
    public RenderFragment<HeaderContext<T>>? HeaderTemplate { get; set; }
    public SortDirection? InitialDirection { get; set; }

    public string PropertyName => (Property.Body as MemberExpression)?.Member.Name ?? string.Empty;
}