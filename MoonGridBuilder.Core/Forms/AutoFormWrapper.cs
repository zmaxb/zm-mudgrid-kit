using FluentValidation;
using MoonGridBuilder.Core.Components;

namespace MoonGridBuilder.Core.Forms;

public static class AutoFormWrapper
{
    public static Type Create<TModel, TValidator>()
        where TValidator : IValidator<TModel>, new()
    {
        return typeof(AutoFormWrapperComponent<TModel, TValidator>);
    }
}