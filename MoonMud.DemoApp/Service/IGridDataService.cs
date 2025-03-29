using MoonMud.DemoApp.Models;
using MudBlazor;

namespace MoonMud.DemoApp.Service;

public interface IGridDataService<T> where T : class
{
    Task<PagedResponse<T>> GetPagedAsync(GridState<T> state);

    Task<GridData<T>> LoadData(GridState<T> state);
}