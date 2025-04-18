using FluentValidation;
using MudBlazor;
using Zm.MudGridKit.Interfaces;

namespace Zm.MudGridKit.DataGrid;

public abstract class AbstractDataGridService<T, TCreateDto, TUpdateDto> : IDataGridService<T>
    where T : class
{
    public IDialogService? DialogService { get; set; }
    
    protected string Search { get; set; } = string.Empty;

    protected virtual IValidator<TCreateDto>? CreateValidator => null;
    protected virtual IValidator<TUpdateDto>? UpdateValidator => null;

    public abstract Task<GridData<T>> LoadData(GridState<T> state);

    public async Task OnAdd()
    {
        if (DialogService is null) return;

        var dialog = await DialogService.ShowAsync(AddDialog, "Add", new DialogOptions { CloseOnEscapeKey = true });
        var result = await dialog.Result;

        if (result is not { Canceled: false, Data: { } createDto }) return;

        if (CreateValidator is not null)
        {
            var validation = await CreateValidator.ValidateAsync((TCreateDto)createDto);
            if (!validation.IsValid) return;
        }

        await OnCreate(createDto);
    }

    public async Task OnEdit(T entity)
    {
        if (DialogService is null) return;

        var dto = MapToUpdateDto(entity);
        var parameters = new DialogParameters { ["Entity"] = dto };

        var dialog = await DialogService.ShowAsync(EditDialog, Title, parameters,
            new DialogOptions { CloseOnEscapeKey = true });
        var result = await dialog.Result;

        if (result is not { Canceled: false, Data: TUpdateDto updateDto }) return;

        if (UpdateValidator is not null)
        {
            var validation = await UpdateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid) return;
        }

        await OnUpdate(entity, updateDto);
    }

    public async Task OnDelete(List<T> items)
    {
        await OnDeleteInternal(items);
    }
    
    public virtual Task OnSearch(string searchText)
    {
        Search = searchText;
        return Task.CompletedTask;
    }
    
    protected abstract Task OnCreate(object createDto);

    protected abstract Task OnUpdate(T existing, object updateDto);

    protected abstract Task OnDeleteInternal(List<T> items);

    protected abstract Type AddDialog { get; }

    protected abstract Type EditDialog { get; }

    protected abstract string Title { get; }

    protected abstract TUpdateDto MapToUpdateDto(T entity);
    
}