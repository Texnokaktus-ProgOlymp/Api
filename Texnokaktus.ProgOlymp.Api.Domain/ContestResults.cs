namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestResults(bool IsFinal, ProblemResult[] Results)
{
    public double TotalScore => Results.Sum(result => result.TotalScore);
}
