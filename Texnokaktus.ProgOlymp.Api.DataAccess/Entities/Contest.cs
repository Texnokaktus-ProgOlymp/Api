namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class Contest
{
    public int Id { get; init; }
    public string Name { get; set; }

    public long? PreliminaryStageId { get; set; }
    public long? FinalStageId { get; set; }

    public DateTimeOffset RegistrationStart { get; set; }
    public DateTimeOffset RegistrationFinish { get; set; }

    public ContestStage? PreliminaryStage { get; set; }
    public ContestStage? FinalStage { get; set; }
}
