using FluentValidation;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Validators;

internal static class DiExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services) =>
        services.AddScoped<IValidator<ApplicationInsertModel>, ApplicationInsertModelValidator>()
                .AddScoped<IValidator<Name>, NameValidator>()
                .AddScoped<IValidator<Teacher>, TeacherValidator>()
                .AddScoped<IValidator<ThirdPerson>, ThirdPersonValidator>();
}
