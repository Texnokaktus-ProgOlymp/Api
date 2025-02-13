namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ProblemResult(string ProblemId, string? ProblemName, double? BaseScore, double? JuryScore)
{
    // public string Title => ProblemName is not null ? $"{ProblemId}: {ProblemName}" : ProblemId;
    public double TotalScore => (BaseScore ?? 0) + (JuryScore ?? 0);
}
