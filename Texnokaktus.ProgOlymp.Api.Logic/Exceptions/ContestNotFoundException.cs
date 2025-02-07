namespace Texnokaktus.ProgOlymp.Api.Logic.Exceptions;

public class ContestNotFoundException(int contestId) : Exception($"Contest with id {contestId} was not found");
