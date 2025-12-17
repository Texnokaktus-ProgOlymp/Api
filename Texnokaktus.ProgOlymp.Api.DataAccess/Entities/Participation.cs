namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class Participation
{
    public long ContestUserId { get; init; }
    public ParticipationState State { get; set; }
    public DateTimeOffset? Start { get; set; }
    public DateTimeOffset? Finish { get; set; }
}
