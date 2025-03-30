using FluentValidation;
using MoonMud.DemoApp.Models;

namespace MoonMud.DemoApp.Validators;

public class AssetUpdateDtoValidator : BaseMudValidator<AssetUpdateDto>
{
    public AssetUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required");

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("Symbol is required")
            .MaximumLength(10).WithMessage("Symbol must be 10 characters max");

        RuleFor(x => x.Precision)
            .GreaterThanOrEqualTo(0).WithMessage("Precision must be >= 0");
    }
}