namespace Texnokaktus.ProgOlymp.Api.Logic.Exceptions;

public class ContestNotFoundException(string contestName) : Exception($"Contest {contestName} was not found");
