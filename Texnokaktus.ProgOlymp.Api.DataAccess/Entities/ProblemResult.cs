namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class ProblemResult
{
    public int Id { get; init; }
    public int ProblemId { get; init; }
    public required int ApplicationId { get; init; }
    public required decimal BaseScore { get; init; }
    public Application Application { get; set; }
    public ICollection<ScoreAdjustment> Adjustments { get; init; }
}
