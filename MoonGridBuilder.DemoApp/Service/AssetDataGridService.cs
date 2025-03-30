using Bogus;
using FluentValidation;
using MoonGridBuilder.Core.DataGrid;
using MoonGridBuilder.Core.Forms;
using MoonMud.DemoApp.Components.Dialogs;
using MoonMud.DemoApp.Models;
using MoonMud.DemoApp.Validators;
using MudBlazor;

namespace MoonMud.DemoApp.Service;

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
        typeof(AddAssetCustomDialog); // Use custom form

    protected override Type EditDialog =>
        AutoFormWrapper.Create<AssetUpdateDto, AssetUpdateDtoValidator>(); // Use AutoForm

    protected override string Title => "Edit Asset";

    protected override IValidator<AssetCreateDto> CreateValidator =>
        new AssetCreateDtoValidator();

    protected override IValidator<AssetUpdateDto> UpdateValidator =>
        new AssetUpdateDtoValidator();
    
    public override Task<GridData<AssetReadDto>> LoadData(GridState<AssetReadDto> state)
    {
        IEnumerable<AssetReadDto> query = Items;

        if (!string.IsNullOrWhiteSpace(Search))
        {
            query = query.Where(x =>
                x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                x.Symbol.Contains(Search, StringComparison.OrdinalIgnoreCase));
        }

        var assetReadDtos = query.ToList();
        var totalItems = assetReadDtos.Count();
        
        var data = assetReadDtos
            .Skip(state.Page * state.PageSize)
            .Take(state.PageSize)
            .ToList();

        return Task.FromResult(new GridData<AssetReadDto>
        {
            Items = data,
            TotalItems = totalItems
        });
    }


    protected override void ApplyUpdate(AssetReadDto existing, object updateDto)
    {
        var dto = (AssetUpdateDto)updateDto;
        existing.Name = dto.Name;
        existing.Symbol = dto.Symbol;
        existing.Precision = dto.Precision;
    }

    protected override AssetReadDto CreateFrom(object createDto)
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

    public override Task OnSearch(string searchText)
    {
        Search = searchText;
        return Task.CompletedTask;
    }

    protected override AssetReadDto? FindById(Guid id)
        => Items.FirstOrDefault(x => x.GlobalId == id);
}