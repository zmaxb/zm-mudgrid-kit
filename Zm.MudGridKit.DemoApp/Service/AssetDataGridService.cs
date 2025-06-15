using Bogus;
using FluentValidation;
using MudBlazor;
using Zm.MudGridKit.DataGrid;
using Zm.MudGridKit.DemoApp.Components.Dialogs;
using Zm.MudGridKit.DemoApp.Models;
using Zm.MudGridKit.DemoApp.Validators;
using Zm.MudGridKit.Forms;

namespace Zm.MudGridKit.DemoApp.Service;

public class AssetDataGridService
    : BaseDataGridService<AssetReadDto, AssetCreateDto, AssetUpdateDto>
{
    public AssetDataGridService()
    {
        var faker = new Faker<AssetReadDto>()
            .RuleFor(x => x.GlobalId, f => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Symbol, f => f.Random.AlphaNumeric(4).ToUpper())
            .RuleFor(x => x.Precision, f => f.Random.Int(0, 8))
            .RuleFor(x => x.CreatedAt, f => f.Date.PastOffset(1).DateTime);

        Items.AddRange(faker.Generate(100));
    }

    protected override Type AddDialog =>
        typeof(AddAssetCustomDialog);

    protected override Type EditDialog =>
        AutoFormWrapper.Create<AssetUpdateDto, AssetUpdateDtoValidator>();
    
    protected override async Task<AssetUpdateDto> MapToUpdateModelAsync(AssetReadDto entity)
    {
        return await Task.FromResult(new AssetUpdateDto
        {
            Name = entity.Name,
            Symbol = entity.Symbol,
            Precision = entity.Precision
        });
    }

    protected override IValidator<AssetCreateDto> CreateValidator =>
        new AssetCreateDtoValidator();

    protected override IValidator<AssetUpdateDto> UpdateValidator =>
        new AssetUpdateDtoValidator();

    protected override AssetReadDto CreateEntityFromDto(object createDto)
    {
        var dto = (AssetCreateDto)createDto;

        return new AssetReadDto
        {
            GlobalId = Guid.NewGuid(),
            Name = dto.Name,
            Symbol = dto.Symbol,
            Precision = dto.Precision,
            CreatedAt = DateTime.Now
        };
    }

    protected override Task ApplyUpdateToEntity(AssetReadDto existing, object updateDto)
    {
        var dto = (AssetUpdateDto)updateDto;

        existing.Name = dto.Name;
        existing.Symbol = dto.Symbol;
        existing.Precision = dto.Precision;

        return Task.CompletedTask;
    }

    public override Task OnSearch(string searchText)
    {
        Search = searchText;
        return Task.CompletedTask;
    }

    public override Task<GridData<AssetReadDto>> LoadData(GridState<AssetReadDto> state)
    {
        IEnumerable<AssetReadDto> query = Items;

        if (!string.IsNullOrWhiteSpace(Search))
        {
            query = query.Where(x =>
                x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                x.Symbol.Contains(Search, StringComparison.OrdinalIgnoreCase));
        }

        var sort = state.SortDefinitions.FirstOrDefault();
        if (sort is not null)
        {
            var prop = typeof(AssetReadDto).GetProperty(sort.SortBy ?? "");
            if (prop is not null)
            {
                query = sort.Descending
                    ? query.OrderByDescending(x => prop.GetValue(x))
                    : query.OrderBy(x => prop.GetValue(x));
            }
        }

        var paged = query
            .Skip(state.Page * state.PageSize)
            .Take(state.PageSize)
            .ToList();

        return Task.FromResult(new GridData<AssetReadDto>
        {
            Items = paged,
            TotalItems = query.Count()
        });
    }

}
