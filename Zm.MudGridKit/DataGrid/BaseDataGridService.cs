using MudBlazor;

namespace Zm.MudGridKit.DataGrid;

public abstract class BaseDataGridService<T, TCreateDto, TUpdateDto>
    : AbstractDataGridService<T, TCreateDto, TUpdateDto>
    where T : class
{
    protected List<T> Items { get; } = [];

    public override Task<GridData<T>> LoadData(GridState<T> state)
    {
        return Task.FromResult(new GridData<T> { Items = Items, TotalItems = Items.Count });
    }

    protected override Task OnCreate(object createDto)
    {
        var entity = CreateEntityFromDto(createDto);
        Items.Add(entity);
        return Task.CompletedTask;
    }

    protected override Task OnUpdate(T existing, object updateDto)
    {
        return ApplyUpdateToEntity(existing, updateDto);
    }

    protected override Task OnDeleteInternal(List<T> items)
    {
        foreach (var item in items)
            Items.Remove(item);

        return OnDeleteEntities(items);
    }

    protected abstract T CreateEntityFromDto(object createDto);

    protected abstract Task ApplyUpdateToEntity(T existing, object updateDto);

    protected virtual Task OnDeleteEntities(List<T> items)
    {
        return Task.CompletedTask;
    }
}