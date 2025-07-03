using MudBlazor;

namespace Zm.MudGridKit.Interfaces;

public interface IReadOnlyDataGridService<T> where T : class
{
    Task<GridData<T>> LoadData(GridState<T> state);
    Task OnSearch(string searchText);
}