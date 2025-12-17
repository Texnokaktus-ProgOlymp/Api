using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.Models;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Services;

public class UserService(AppDbContext context) : IUserService
{
    public async Task<Results<Ok<UserModel>, NotFound>> GetUserAsync(int userId) =>
        await context.Users
                     .Where(user => user.Id == userId)
                     .Select(user => new UserModel(user.Login, user.DisplayName, user.DefaultAvatar))
                     .FirstOrDefaultAsync() is { } userModel
            ? TypedResults.Ok(userModel)
            : TypedResults.NotFound();
}
