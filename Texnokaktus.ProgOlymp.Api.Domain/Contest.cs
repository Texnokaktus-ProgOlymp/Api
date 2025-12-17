namespace Texnokaktus.ProgOlymp.Api.Domain;

public record Contest
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required DateTimeOffset RegistrationStart { get; init; }
    public required DateTimeOffset RegistrationFinish { get; init; }
    public ContestStage? PreliminaryStage { get; init; }
    public ContestStage? FinalStage { get; init; }
}
