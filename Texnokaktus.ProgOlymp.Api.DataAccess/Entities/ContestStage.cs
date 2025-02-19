namespace Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

public class ContestStage
{
    public long Id { get; set; }
    public required string Name { get; set; }

    public DateTimeOffset ContestStart { get; set; }
    public DateTimeOffset? ContestFinish { get; set; }

    public TimeSpan Duration { get; set; }
    
    // public Contest? Contest { get; set; }
}
