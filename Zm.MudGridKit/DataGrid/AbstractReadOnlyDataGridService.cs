using MudBlazor;
using Zm.MudGridKit.Interfaces;

namespace Zm.MudGridKit.DataGrid;

public abstract class AbstractReadOnlyDataGridService<T> : IReadOnlyDataGridService<T>
    where T : class
{
    protected string? Search { get; private set; }

    public virtual Task OnSearch(string searchText)
    {
        Search = searchText;
        return Task.CompletedTask;
    }

    public abstract Task<GridData<T>> LoadData(GridState<T> state);
}