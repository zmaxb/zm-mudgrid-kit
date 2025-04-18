namespace Zm.MudGridKit.DataGrid;

public abstract class ApiDataGridService<T, TCreateDto, TUpdateDto>
    : AbstractDataGridService<T, TCreateDto, TUpdateDto>
    where T : class
{
    protected override Task OnCreate(object createDto)
        => CreateEntityFromDto(createDto);

    protected override Task OnUpdate(T existing, object updateDto)
        => ApplyUpdateToEntity(existing, updateDto);

    protected override Task OnDeleteInternal(List<T> items)
        => DeleteEntities(items);

    protected abstract Task CreateEntityFromDto(object createDto);

    protected abstract Task ApplyUpdateToEntity(T existing, object updateDto);

    protected abstract Task DeleteEntities(List<T> items);
}