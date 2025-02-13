using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Services.Abstractions;

public interface IUserService
{
    Task<Results<Ok<UserModel>, NotFound>> GetUserAsync(int userId);
}
