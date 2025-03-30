using MoonGridBuilder.Core;
using MoonMud.DemoApp.Components.Dialogs;
using MoonMud.DemoApp.Models;
using FluentValidation;
using MoonMud.DemoApp.Validators;

namespace MoonMud.DemoApp.Service;

public class AssetDataGridService
    : BaseDataGridService<AssetReadDto, AssetCreateDto, AssetUpdateDto>
{
    public AssetDataGridService()
    {
        Items.AddRange([
            new AssetReadDto
            {
                GlobalId = Guid.NewGuid(),
                Name = "Asset 1",
                Type = "Type 1",
                Symbol = "SYM1",
                Precision = 2,
                CreatedAt = DateTime.Now
            },
            new AssetReadDto
            {
                GlobalId = Guid.NewGuid(),
                Name = "Asset 2",
                Type = "Type 2",
                Symbol = "SYM2",
                Precision = 3,
                CreatedAt = DateTime.Now.AddMinutes(-5)
            }
        ]);
    }

    protected override Type AddDialog => typeof(AddAssetDialog);
    protected override Type EditDialog => typeof(EditAssetDialog);
    protected override string Title => "Edit Asset";

    protected override IValidator<AssetCreateDto>? CreateValidator => new AssetCreateDtoValidator();
    protected override IValidator<AssetUpdateDto>? UpdateValidator => new AssetUpdateDtoValidator();

    protected override void ApplyUpdate(AssetReadDto existing, object updateDto)
    {
        var dto = (AssetUpdateDto)updateDto;

        existing.Name = dto.Name;
        existing.Type = dto.Type;
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
            Type = dto.Type,
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