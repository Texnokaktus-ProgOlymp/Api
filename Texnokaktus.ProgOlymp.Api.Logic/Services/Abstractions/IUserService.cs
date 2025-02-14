using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Exceptions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    async Task<User> GetRequiredUserAsync(int id) => await GetByIdAsync(id) ?? throw new UserNotFoundException(id);
    Task<User> AuthenticateUserAsync(string code);
}
