using FluentValidation;
using MoonMud.DemoApp.Models;

namespace MoonMud.DemoApp.Validators;

public class AssetCreateDtoValidator : BaseMudValidator<AssetCreateDto>
{
    public AssetCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required");

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("Symbol is required");

        RuleFor(x => x.Precision)
            .GreaterThanOrEqualTo(0).WithMessage("Precision must be zero or positive");
    }
}