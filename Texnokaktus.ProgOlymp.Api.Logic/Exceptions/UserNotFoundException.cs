namespace Texnokaktus.ProgOlymp.Api.Logic.Exceptions;

public class UserNotFoundException(int userId) : Exception($"User with id {userId} was not find");
