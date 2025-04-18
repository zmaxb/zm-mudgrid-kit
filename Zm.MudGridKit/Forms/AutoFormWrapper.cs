using FluentValidation;
using Zm.MudGridKit.Components;

namespace Zm.MudGridKit.Forms;

public static class AutoFormWrapper
{
    public static Type Create<TModel, TValidator>()
        where TValidator : IValidator<TModel>, new()
    {
        return typeof(AutoFormWrapperComponent<TModel, TValidator>);
    }
}