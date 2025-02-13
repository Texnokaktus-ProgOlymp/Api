using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Models;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Services;

public class UserService(Logic.Services.Abstractions.IUserService userService) : IUserService
{
    public async Task<Results<Ok<UserModel>, NotFound>> GetUserAsync(int userId) =>
        await userService.GetByIdAsync(userId) is { } user
            ? TypedResults.Ok(new UserModel(user.Login, user.DisplayName, user.DefaultAvatar))
            : TypedResults.NotFound();
}
