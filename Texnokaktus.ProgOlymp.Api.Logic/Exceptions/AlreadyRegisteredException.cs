namespace Texnokaktus.ProgOlymp.Api.Logic.Exceptions;

public class AlreadyRegisteredException(string contestName, int userId) : Exception($"User {userId} is already registered to contest {contestName}");
