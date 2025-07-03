using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Zm.MudGridKit.DataGrid;

namespace Zm.MudGridKit.Components;

public abstract class ZmMudDataGridBase<T> : ComponentBase where T : class
{
    protected MudDataGrid<T> DataGrid = null!;

    protected IStringLocalizer L = null!;
    [Parameter] public DataGridOptions Options { get; set; } = new();
    [Parameter] public RenderFragment? Columns { get; set; }
    [Parameter] public RenderFragment? TopRightContent { get; set; }
    protected abstract Task<GridData<T>> LoadData(GridState<T> state);
    protected abstract Task OnSearch(string? searchText);

    protected async Task<GridData<T>> WrappedServerLoadData(GridState<T> state)
    {
        DataGrid.SelectedItems.Clear();
        return await LoadData(state);
    }

    public async Task ReloadAsync()
    {
        await DataGrid.ReloadServerData();
    }
}