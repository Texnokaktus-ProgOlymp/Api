namespace Texnokaktus.ProgOlymp.Api.Domain;

public record ParticipantData(Name Name, DateOnly BirthDate, string? Snils, bool IsSnilsValid, string Email, string School, string Region, int Grade);
