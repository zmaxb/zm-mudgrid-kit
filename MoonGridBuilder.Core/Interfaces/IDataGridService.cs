using MudBlazor;

namespace MoonGridBuilder.Core.Interfaces;

public interface IDataGridService<T> where T : class
{
    IDialogService? DialogService { get; set; }
    Task<GridData<T>> LoadData(GridState<T> state);
    Task OnSearch(string searchText);
    Task OnAdd();
    Task OnEdit(T entity);
    Task OnDelete(List<T> items);
}