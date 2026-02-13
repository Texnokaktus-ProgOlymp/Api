namespace Texnokaktus.ProgOlymp.Api.Logic.Exceptions;

public class RegistrationClosedException(string contestName) : Exception($"Registration to contest {contestName} is closed");
