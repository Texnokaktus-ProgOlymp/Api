namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class Problem
{
    public int Id { get; init; }
    public int ContestResultId { get; init; }
    public required string Alias { get; init; }
    public required string Name { get; init; }
    public ICollection<ProblemResult> Results { get; init; }
}
