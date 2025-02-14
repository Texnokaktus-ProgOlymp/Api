using FluentValidation;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Validators;

public class TeacherValidator : AbstractValidator<Teacher>
{
    public TeacherValidator(IValidator<Name> nameValidator)
    {
        RuleFor(teacher => teacher.Name).SetValidator(nameValidator);
        RuleFor(teacher => teacher.Email).EmailAddress().When(person => person.Email is not null);
        RuleFor(teacher => teacher.School).NotEmpty();
    }
}
