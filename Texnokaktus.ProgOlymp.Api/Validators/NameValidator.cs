using FluentValidation;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Validators;

public class NameValidator : AbstractValidator<Name>
{
    public NameValidator()
    {
        RuleFor(name => name.FirstName).NotEmpty();
        RuleFor(name => name.LastName).NotEmpty();
        RuleFor(name => name.Patronym).NotEmpty().When(name => name.Patronym is not null);
    }
}
