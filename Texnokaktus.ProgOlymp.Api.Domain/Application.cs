namespace Texnokaktus.ProgOlymp.Api.Domain;

public record Application
{
    public required int Id { get; init; }
    public required Guid? Uid { get; init; }
    public required User User { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required ParticipantData ParticipantData { get; init; }
    public required ParentData ParentData { get; init; }
    public required TeacherData TeacherData { get; init; }
    public required bool PersonalDataConsent { get; init; }
}
