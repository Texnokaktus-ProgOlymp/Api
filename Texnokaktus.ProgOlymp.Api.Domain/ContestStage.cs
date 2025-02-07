namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestStage(long Id,
                           string Name,
                           DateTimeOffset ContestStart,
                           DateTimeOffset? ContestFinish,
                           TimeSpan Duration);
