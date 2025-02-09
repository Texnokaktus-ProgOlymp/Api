using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class UserServiceCachingDecorator(IUserService userService, IMemoryCache memoryCache) : IUserService
{
    public Task<User?> GetByIdAsync(int id) =>
        memoryCache.GetOrCreateAsync(GetKey(id),
                                     entry =>
                                     {
                                         entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                                         return userService.GetByIdAsync(id);
                                     });

    public async Task<User> AuthenticateUserAsync(string code)
    {
        var user = await userService.AuthenticateUserAsync(code);
        memoryCache.Remove(GetKey(user.Id));

        return user;
    }

    private static string GetKey(int userId) => $"Users:{userId}";
}
