using FluentValidation;
using MoonGridBuilder.Core.Interfaces;
using MudBlazor;

namespace MoonGridBuilder.Core.DataGrid;

public abstract class BaseDataGridService<T, TCreateDto, TUpdateDto> : IDataGridService<T>
    where T : class
{
    protected List<T> Items { get; } = new();
    protected string Search = string.Empty;

    public IDialogService? DialogService { get; set; }

    protected virtual IValidator<TCreateDto>? CreateValidator => null;
    protected virtual IValidator<TUpdateDto>? UpdateValidator => null;

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

        if (result is not { Canceled: false, Data: { } createDto })
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

        var dto = MapToUpdateDto(entity);

        var parameters = new DialogParameters
        {
            ["Entity"] = dto
        };

        var dialog = await DialogService.ShowAsync(EditDialog, Title, parameters,
            new DialogOptions { CloseOnEscapeKey = true });
        var result = await dialog.Result;

        if (result is not { Canceled: false, Data: TUpdateDto updateDto })
            return;

        if (UpdateValidator is not null)
        {
            var validation = await UpdateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return;
        }

        ApplyUpdate(entity, updateDto);
    }

    public virtual Task OnDelete(List<T> items)
    {
        foreach (var item in items)
            Items.Remove(item);

        return Task.CompletedTask;
    }

    protected virtual TUpdateDto MapToUpdateDto(T entity)
    {
        var updateDto = Activator.CreateInstance<TUpdateDto>();
        var sourceProps = typeof(T).GetProperties();
        var targetProps = typeof(TUpdateDto).GetProperties();

        foreach (var targetProp in targetProps)
        {
            var sourceProp = sourceProps.FirstOrDefault(p =>
                p.Name == targetProp.Name &&
                p.PropertyType == targetProp.PropertyType);

            if (sourceProp != null)
            {
                var value = sourceProp.GetValue(entity);
                targetProp.SetValue(updateDto, value);
            }
        }

        return updateDto!;
    }

    protected abstract Type AddDialog { get; }
    protected abstract Type EditDialog { get; }
    protected abstract string Title { get; }
    protected abstract void ApplyUpdate(T existing, object updateDto);
    protected abstract T CreateFrom(object createDto);
    protected abstract T? FindById(Guid id);
}