using Zm.MudGridKit.DataGrid;

namespace Zm.MudGridKit.DemoApp.Components.Extensions;

public static class ColumnConfigExtensions
{
    public static ColumnConfig<T> Centered<T>(this ColumnConfig<T> config)
    {
        config.CellStyle = "text-align: center";
        config.HeaderStyle = "text-align: center";
        return config;
    }
}