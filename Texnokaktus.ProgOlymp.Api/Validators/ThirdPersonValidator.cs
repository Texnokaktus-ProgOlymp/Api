using FluentValidation;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Validators;

public class ThirdPersonValidator : AbstractValidator<ThirdPerson>
{
    public ThirdPersonValidator(IValidator<Name> nameValidator)
    {
        RuleFor(person => person.Name).SetValidator(nameValidator);
        RuleFor(person => person.Email).EmailAddress().When(person => person.Email is not null);
    }
}
