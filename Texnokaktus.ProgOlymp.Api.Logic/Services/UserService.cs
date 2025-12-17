using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Observability;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class UserService(AppDbContext context, IYandexIdUserServiceClient yandexIdUserServiceClient, TimeProvider timeProvider) : IUserService
{
    private readonly Counter<int> _authenticatedUsersCounter = MeterProvider.Meter.CreateAuthenticatedUsersCounter();

    public async Task<User?> GetByIdAsync(int id) =>
        await context.Users
                     .Where(user => user.Id == id)
                     .Select(user => user.MapUser())
                     .FirstOrDefaultAsync();

    public async Task<User> AuthenticateUserAsync(string code)
    {
        var user = await yandexIdUserServiceClient.AuthenticateUserAsync(code);

        var dbUser = await context.Users.FirstOrDefaultAsync(u => u.Login == user.Login)
                  ?? context.Users
                            .Add(new()
                             {
                                 Login = user.Login,
                                 DisplayName = user.DisplayName,
                                 DefaultAvatar = user.Avatar?.AvatarId,
                                 Created = timeProvider.GetUtcNow()
                             })
                            .Entity;

        dbUser.DisplayName = user.DisplayName;
        dbUser.DefaultAvatar = user.Avatar?.AvatarId;
        await context.SaveChangesAsync();

        _authenticatedUsersCounter.Add(1, KeyValuePair.Create<string, object?>("login", user.Login));

        return dbUser.MapUser();
    }
}

file static class MappingExtensions
{
    public static User MapUser(this DataAccess.Entities.User user) =>
        new()
        {
            Id = user.Id,
            Login = user.Login,
            DisplayName = user.DisplayName,
            DefaultAvatar = user.DefaultAvatar
        };
}
