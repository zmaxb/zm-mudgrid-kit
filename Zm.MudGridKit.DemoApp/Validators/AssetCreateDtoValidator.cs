using FluentValidation;
using Zm.MudGridKit.DemoApp.Models;
using Zm.MudGridKit.Forms;

namespace Zm.MudGridKit.DemoApp.Validators;

public class AssetCreateDtoValidator : BaseGridValidator<AssetCreateDto>
{
    public AssetCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long");

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("Symbol is required");

        RuleFor(x => x.Precision)
            .GreaterThanOrEqualTo(0).WithMessage("Precision must be zero or positive");
    }
}