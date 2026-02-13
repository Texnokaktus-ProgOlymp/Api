namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestRegistrationState
{
    public required int ContestId { get; init; }
    public required string ContestName { get; init; }
    public required DateTimeOffset RegistrationStart { get; init; }
    public required DateTimeOffset RegistrationFinish { get; init; }
    public required RegistrationState State { get; init; }
}
