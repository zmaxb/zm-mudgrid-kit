using FluentValidation;
using MoonGridBuilder.Core.Interfaces;
using MudBlazor;

namespace MoonGridBuilder.Core;

public abstract class BaseDataGridService<T, TCreateDto, TUpdateDto> : IDataGridService<T>
    where T : class
{
    protected List<T> Items { get; } = new();
    protected string Search = string.Empty;

    public IDialogService? DialogService { get; set; }

    protected virtual IValidator<TCreateDto>? CreateValidator => null;
    protected virtual IValidator<TUpdateDto>? UpdateValidator => null;

    private static readonly DialogOptions _dialogOptions = new()
    {
        CloseOnEscapeKey = true
    };

    public virtual Task OnSearch(string searchText)
    {
        Search = searchText;
        return Task.CompletedTask;
    }

    public virtual Task<GridData<T>> LoadData(GridState<T> state)
    {
        return Task.FromResult(new GridData<T>
        {
            Items = Items,
            TotalItems = Items.Count
        });
    }

    public async Task OnAdd()
    {
        if (DialogService is null) return;

        var dialog = await DialogService.ShowAsync(AddDialog, "Add", new DialogOptions { CloseOnEscapeKey = true });
        var result = await dialog.Result;

        if (result.Canceled || result.Data is not object createDto)
            return;

        if (CreateValidator is not null)
        {
            var validation = await CreateValidator.ValidateAsync((TCreateDto)createDto);
            if (!validation.IsValid)
                return;
        }

        var entity = CreateFrom(createDto);
        Items.Add(entity);
    }

    
    public async Task OnEdit(T entity)
    {
        if (DialogService is null) return;

        var parameters = new DialogParameters { ["Entity"] = entity };
        var dialog = await DialogService.ShowAsync(EditDialog, Title, parameters, new DialogOptions { CloseOnEscapeKey = true });
        var result = await dialog.Result;

        if (result.Canceled || result.Data is not (Guid id, object updateDto))
            return;

        if (UpdateValidator is not null)
        {
            var validation = await UpdateValidator.ValidateAsync((TUpdateDto)updateDto);
            if (!validation.IsValid)
                return;
        }


        var existing = FindById(id);
        if (existing != null)
            ApplyUpdate(existing, updateDto);
    }



    public virtual Task OnDelete(List<T> items)
    {
        foreach (var item in items)
            Items.Remove(item);

        return Task.CompletedTask;
    }

    protected abstract Type AddDialog { get; }
    protected abstract Type EditDialog { get; }
    protected abstract string Title { get; }
    protected abstract void ApplyUpdate(T existing, object updateDto);
    protected abstract T CreateFrom(object createDto);
    protected abstract T? FindById(Guid id);
}