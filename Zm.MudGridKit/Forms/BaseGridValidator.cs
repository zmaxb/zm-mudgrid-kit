using FluentValidation;

namespace Zm.MudGridKit.Forms;

public abstract class BaseGridValidator<T> : AbstractValidator<T>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            var context = ValidationContext<T>.CreateWithOptions(
                (T)model,
                options => options.IncludeProperties(propertyName));

            var result = await ValidateAsync(context);

            return result.IsValid
                ? Array.Empty<string>()
                : result.Errors.Select(e => e.ErrorMessage);
        };
}