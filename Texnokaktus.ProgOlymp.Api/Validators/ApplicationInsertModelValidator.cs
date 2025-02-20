using System.Text.RegularExpressions;
using FluentValidation;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Validators;

public class ApplicationInsertModelValidator : AbstractValidator<ApplicationInsertModel>
{
    public ApplicationInsertModelValidator(IRegionService regionService,
                                           IValidator<Name> nameValidator,
                                           IValidator<ThirdPerson> thirdPersonValidator,
                                           IValidator<Teacher> teacherValidator)
    {
        RuleFor(model => model.Name).SetValidator(nameValidator);
        // RuleFor(model => model.Snils).Matches(SnilsRegex());
        RuleFor(model => model.Email).EmailAddress();
        RuleFor(model => model.SchoolName).NotEmpty();
        RuleFor(model => model.RegionId).MustAsync((regionId, _) => regionService.ExistsAsync(regionId));
        RuleFor(model => model.Parent).SetValidator(thirdPersonValidator);
        RuleFor(model => model.Teacher).SetValidator(teacherValidator);
        RuleFor(model => model.PersonalDataConsent).Equal(true);
        RuleFor(model => model.Grade).InclusiveBetween(8, 11);
    }

    /*
    [GeneratedRegex(@"^\d{3}-\d{3}-\d{3} \d{2}$")]
    private static partial Regex SnilsRegex();
    */
}
