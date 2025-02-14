namespace Texnokaktus.ProgOlymp.Api.Services.Abstractions;

public interface IResultService
{
    Task GetParticipationAsync(int contestId, int userId);
}
