using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MoonMud.Core;

public class ColumnBuilder<TItem>
{
    private readonly List<ColumnConfig<TItem>> _columns = new();

    public static ColumnBuilder<TItem> For() => new();

    public ColumnBuilder<TItem> Add<TProp>(
        Expression<Func<TItem, TProp>> property,
        string title,
        bool sortable = true,
        bool filterable = true,
        Align align = Align.Left,
        string? format = null,
        bool visible = true)
    {
        _columns.Add(new ColumnConfig<TItem>
        {
            Property = property,
            Title = title,
            Sortable = sortable,
            Filterable = filterable,
            Align = align,
            Format = format,
            Visible = visible,
            PropertyType = typeof(TProp)
        });

        return this;
    }

    public ColumnBuilder<TItem> AddColumn(Action<ColumnConfig<TItem>> configure)
    {
        var config = new ColumnConfig<TItem>();
        configure(config);

        if (config.Property == null)
            throw new InvalidOperationException("Column must have a Property defined.");

        config.PropertyType ??= GetPropertyType(config.Property);
        _columns.Add(config);

        return this;
    }

    public ColumnBuilder<TItem> AddColumn<TProp>(
        Expression<Func<TItem, TProp>> property,
        Action<ColumnConfig<TItem>> configure)
    {
        var config = new ColumnConfig<TItem>
        {
            Property = property,
            PropertyType = typeof(TProp)
        };

        configure(config);
        _columns.Add(config);

        return this;
    }

    private static Type GetPropertyType(LambdaExpression expr)
    {
        var member = expr.Body as MemberExpression;
        var propertyInfo = member?.Member as System.Reflection.PropertyInfo;
        return propertyInfo?.PropertyType ?? typeof(object);
    }

    public ColumnBuilder<TItem> WithTemplate<TProp>(
        Expression<Func<TItem, TProp>> property,
        RenderFragment<CellContext<TItem>> template)
    {
        var column = FindColumn(property);
        if (column != null)
            column.Template = template;

        return this;
    }

    public ColumnBuilder<TItem> WithHeaderTemplate<TProp>(
        Expression<Func<TItem, TProp>> property,
        RenderFragment<HeaderContext<TItem>> template)
    {
        var column = FindColumn(property);
        if (column != null)
            column.HeaderTemplate = template;

        return this;
    }

    public RenderFragment Build()
    {
        return builder =>
        {
            int seq = 0;

            foreach (var column in _columns)
            {
                if (!column.Visible) continue;

                var componentType = typeof(PropertyColumn<,>)
                    .MakeGenericType(typeof(TItem), column.PropertyType);

                builder.OpenComponent(seq++, componentType);
                builder.AddAttribute(seq++, "Property", column.Property);

                if (column.HeaderTemplate is not null)
                {
                    builder.AddAttribute(seq++, "HeaderTemplate", column.HeaderTemplate);
                }
                else
                {
                    builder.AddAttribute(seq++, "Title", column.Title);
                }

                builder.AddAttribute(seq++, "Sortable", column.Sortable);
                builder.AddAttribute(seq++, "Filterable", column.Filterable);
                builder.AddAttribute(seq++, "Align", column.Align);

                if (!string.IsNullOrWhiteSpace(column.Format))
                    builder.AddAttribute(seq++, "Format", column.Format);

                if (column.Template is not null)
                {
                    builder.AddAttribute(seq++, "CellTemplate", column.Template);
                }

                builder.CloseComponent();
            }
        };
    }

    private ColumnConfig<TItem>? FindColumn<TProp>(Expression<Func<TItem, TProp>> property)
    {
        var targetName = (property.Body as MemberExpression)?.Member.Name;
        return _columns.FirstOrDefault(c => c.PropertyName == targetName);
    }

    public class ColumnConfig<T>
    {
        public LambdaExpression Property { get; set; } = default!;
        public string Title { get; set; } = string.Empty;
        public bool Sortable { get; set; } = true;
        public bool Filterable { get; set; } = true;
        public Align Align { get; set; } = Align.Left;
        public string? Format { get; set; }
        public bool Visible { get; set; } = true;
        public Type PropertyType { get; set; } = default!;
        public RenderFragment<CellContext<TItem>>? Template { get; set; }
        public RenderFragment<HeaderContext<TItem>>? HeaderTemplate { get; set; }

        public string PropertyName => (Property.Body as MemberExpression)?.Member.Name ?? string.Empty;
    }
}