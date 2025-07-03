using MudBlazor;

namespace Zm.MudGridKit.Interfaces;

public interface ICrudDataGridService<T> : IReadOnlyDataGridService<T>
    where T : class
{
    IDialogService? DialogService { get; set; }

    Task OnAdd();
    Task OnEdit(T entity);
    Task OnDelete(List<T> items);
}