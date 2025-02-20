namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ContestApplications(Contest Contest, IEnumerable<Application> Applications);
