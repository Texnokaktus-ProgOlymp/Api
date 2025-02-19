using System.Diagnostics.Metrics;
using Texnokaktus.ProgOlymp.Api.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Observability;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class UserService(IUnitOfWork unitOfWork, IYandexIdUserServiceClient yandexIdUserServiceClient) : IUserService
{
    private readonly Counter<int> _authenticatedUsersCounter = MeterProvider.Meter.CreateAuthenticatedUsersCounter();

    public async Task<User?> GetByIdAsync(int id)
    {
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(id);
        return user?.MapUser();
    }

    public async Task<User> AuthenticateUserAsync(string code)
    {
        var user = await yandexIdUserServiceClient.AuthenticateUserAsync(code);
        var dbUser = await ConvertToDbUserAsync(user);
        await unitOfWork.SaveChangesAsync();
        _authenticatedUsersCounter.Add(1, KeyValuePair.Create<string, object?>("login", user.Login));

        return dbUser.MapUser();
    }

    private async Task<DataAccess.Entities.User> ConvertToDbUserAsync(Common.Contracts.Grpc.YandexId.User user)
    {
        if (await unitOfWork.UserRepository.GetUserByLoginAsync(user.Login) is not { } dbUser)
            return unitOfWork.UserRepository.AddUser(new(user.Login, user.DisplayName, user.Avatar?.AvatarId));

        dbUser.DisplayName = user.DisplayName;
        dbUser.DefaultAvatar = user.Avatar?.AvatarId;
        return dbUser;
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
