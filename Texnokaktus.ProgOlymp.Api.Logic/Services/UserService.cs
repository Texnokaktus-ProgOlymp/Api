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
        var dbUser = await ConvertToDbUserAsync(user);
        await context.SaveChangesAsync();
        _authenticatedUsersCounter.Add(1, KeyValuePair.Create<string, object?>("login", user.Login));

        return dbUser.MapUser();
    }

    private async Task<DataAccess.Entities.User> ConvertToDbUserAsync(Common.Contracts.Grpc.YandexId.User grpcUser)
    {
        var user = await context.Users.FirstOrDefaultAsync(user => user.Login == grpcUser.Login)
                ?? context.Users
                          .Add(new()
                           {
                               Login = grpcUser.Login,
                               DisplayName = grpcUser.DisplayName,
                               DefaultAvatar = grpcUser.Avatar?.AvatarId,
                               Created = timeProvider.GetUtcNow()
                           })
                          .Entity;

        user.DisplayName = grpcUser.DisplayName;
        user.DefaultAvatar = grpcUser.Avatar?.AvatarId;
        return user;
    }
}

file static class MappingExtensions
{
    public static User MapUser(this DataAccess.Entities.User user) =>
        new(user.Id,
            user.Login,
            user.DisplayName,
            user.DefaultAvatar);
}
