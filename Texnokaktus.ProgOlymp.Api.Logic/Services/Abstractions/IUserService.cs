using Texnokaktus.ProgOlymp.Api.Logic.Models;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IUserService
{
    Task<bool> IsUserRegisteredAsync(int contestId, string login);
    Task<int> RegisterUserAsync(UserInsertModel userInsertModel);
}
