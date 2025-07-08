using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Zm.MudGridKit.DataGrid;

public class ColumnBuilder<TItem>
{
    private readonly List<ColumnConfig<TItem>> _columns = new();
    private ColumnConfig<TItem>? _selectionColumn;

    public static ColumnBuilder<TItem> For()
    {
        return new ColumnBuilder<TItem>();
    }

    public ColumnBuilder<TItem> AddColumn(Action<ColumnConfig<TItem>> configure)
    {
        var config = new ColumnConfig<TItem>();
        configure(config);

        if (config.Property == null && !config.IsSelectionColumn)
            throw new InvalidOperationException("Column must have a Property defined.");

        _columns.Add(config);
        return this;
    }

    public ColumnBuilder<TItem> AddColumn<TProp>(Expression<Func<TItem, TProp>> property,
        Action<ColumnConfig<TItem>>? configure)
    {
        var config = new ColumnConfig<TItem>
        {
            Property = property,
            PropertyType = typeof(TProp)
        };

        configure?.Invoke(config);
        _columns.Add(config);
        return this;
    }

    public ColumnBuilder<TItem> AddColumn<TProp>(Expression<Func<TItem, TProp>> property)
    {
        return AddColumn(property, null);
    }

    public ColumnBuilder<TItem> AddColumnsIf(bool condition, Action<ColumnBuilder<TItem>> columns)
    {
        if (condition) columns(this);
        return this;
    }

    public ColumnBuilder<TItem> WithTemplate<TProp>(Expression<Func<TItem, TProp>> property,
        RenderFragment<CellContext<TItem>> template)
    {
        var column = FindColumn(property);
        if (column != null)
            column.Template = template;

        return this;
    }

    public ColumnBuilder<TItem> WithHeaderTemplate<TProp>(Expression<Func<TItem, TProp>> property,
        RenderFragment<HeaderContext<TItem>> template)
    {
        var column = FindColumn(property);
        if (column != null)
            column.HeaderTemplate = template;

        return this;
    }

    public ColumnBuilder<TItem> WithRowSelection(Action<ColumnConfig<TItem>>? configure = null)
    {
        _selectionColumn = new ColumnConfig<TItem>
        {
            IsSelectionColumn = true,
            Property = (Expression<Func<TItem, bool>>)(x => true),
            PropertyType = typeof(bool)
        };

        configure?.Invoke(_selectionColumn);
        return this;
    }

    public RenderFragment Build()
    {
        return builder =>
        {
            var seq = 0;

            // всегда сначала чекбокс
            if (_selectionColumn != null && _selectionColumn.Visible)
                BuildColumnComponent(builder, ref seq, _selectionColumn);

            foreach (var column in _columns.Where(c => c.Visible)) BuildColumnComponent(builder, ref seq, column);
        };
    }

    private void BuildColumnComponent(RenderTreeBuilder builder, ref int seq, ColumnConfig<TItem> column)
    {
        var isSelection = column.IsSelectionColumn;
        var componentType = isSelection
            ? typeof(SelectColumn<>).MakeGenericType(typeof(TItem))
            : typeof(PropertyColumn<,>).MakeGenericType(typeof(TItem), column.PropertyType);

        builder.OpenComponent(seq++, componentType);

        if (!isSelection)
        {
            builder.AddAttribute(seq++, "Property", column.Property);
            builder.AddAttribute(seq++, column.HeaderTemplate is not null ? "HeaderTemplate" : "Title",
                column.HeaderTemplate ?? (object?)column.Title);
            builder.AddAttribute(seq++, "Sortable", column.Sortable);
            builder.AddAttribute(seq++, "Filterable", column.Filterable);

            if (!string.IsNullOrWhiteSpace(column.Format))
                builder.AddAttribute(seq++, "Format", column.Format);

            if (column.InitialDirection is not null)
                builder.AddAttribute(seq++, "InitialDirection", column.InitialDirection);
        }

        if (!string.IsNullOrWhiteSpace(column.CellStyle))
            builder.AddAttribute(seq++, "CellStyle", column.CellStyle);

        if (!string.IsNullOrWhiteSpace(column.HeaderStyle))
            builder.AddAttribute(seq++, "HeaderStyle", column.HeaderStyle);

        if (column.Template is not null)
            builder.AddAttribute(seq++, "CellTemplate", column.Template);

        builder.CloseComponent();
    }

    private ColumnConfig<TItem>? FindColumn<TProp>(Expression<Func<TItem, TProp>> property)
    {
        var targetName = (property.Body as MemberExpression)?.Member.Name;
        return _columns.FirstOrDefault(c => c.PropertyName == targetName);
    }
}