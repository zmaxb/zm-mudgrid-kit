using FluentValidation;
using MudBlazor;
using Zm.MudGridKit.Interfaces;

namespace Zm.MudGridKit.DataGrid;

public abstract class AbstractCrudDataGridService<T, TCreateDto, TUpdateDto> : ICrudDataGridService<T>
    where T : class
{
    protected string Search { get; set; } = string.Empty;

    protected virtual IValidator<TCreateDto>? CreateValidator => null;
    protected virtual IValidator<TUpdateDto>? UpdateValidator => null;

    protected abstract Type AddDialog { get; }
    protected abstract Type EditDialog { get; }

    protected virtual DialogOptions DefaultDialogOptions => new()
    {
        CloseOnEscapeKey = true,
        FullWidth = true,
        MaxWidth = MaxWidth.Medium,
        CloseButton = true
    };

    protected virtual DialogOptions AddDialogOptions => DefaultDialogOptions;

    protected virtual DialogOptions EditDialogOptions => DefaultDialogOptions;
    public IDialogService? DialogService { get; set; }

    public abstract Task<GridData<T>> LoadData(GridState<T> state);

    public virtual Task OnSearch(string searchText)
    {
        Search = searchText;
        return Task.CompletedTask;
    }

    public async Task OnAdd()
    {
        if (DialogService is null) return;

        var dialog = await DialogService.ShowAsync(AddDialog, "Add", AddDialogOptions);
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

        var dto = await MapToUpdateModelAsync(entity);
        var parameters = new DialogParameters { ["Entity"] = dto };

        var dialog = await DialogService.ShowAsync(EditDialog, "Edit", parameters, EditDialogOptions);
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

    protected abstract Task OnCreate(object createDto);
    protected abstract Task OnUpdate(T existing, object updateDto);
    protected abstract Task OnDeleteInternal(List<T> items);

    protected abstract Task<TUpdateDto> MapToUpdateModelAsync(T entity);
}