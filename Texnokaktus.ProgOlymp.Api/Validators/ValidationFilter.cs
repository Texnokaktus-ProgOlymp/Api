using FluentValidation;

namespace Texnokaktus.ProgOlymp.Api.Validators;

public class ValidationFilter<TModel>(IValidator<TModel> validator) : IEndpointFilter where TModel : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.OfType<TModel>().FirstOrDefault() is not { } model)
            return await next(context);

        var validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        return await next(context);
    }
}
