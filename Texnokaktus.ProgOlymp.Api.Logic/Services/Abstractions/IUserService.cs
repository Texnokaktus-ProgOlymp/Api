using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    Task<User> AuthenticateUserAsync(string code);
}
