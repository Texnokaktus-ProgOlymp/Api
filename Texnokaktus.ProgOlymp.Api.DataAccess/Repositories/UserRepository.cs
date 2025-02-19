using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.DataAccess.Models;
using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User[]> GetUsersAsync() =>
        context.Users
               .AsNoTracking()
               .ToArrayAsync();

    public Task<User?> GetUserByIdAsync(int id) =>
        context.Users
               .AsNoTracking()
               .FirstOrDefaultAsync(user => user.Id == id);

    public Task<User?> GetUserByLoginAsync(string login) =>
        context.Users
               .FirstOrDefaultAsync(user => user.Login == login);

    public User AddUser(UserInsertModel insertModel)
    {
        var entity = new User
        {
            Login = insertModel.Login,
            DisplayName = insertModel.DisplayName,
            DefaultAvatar = insertModel.DefaultAvatar
        };

        return context.Users.Add(entity).Entity;
    }

    public Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate) =>
        context.Users
               .AsNoTracking()
               .AnyAsync(predicate);
}
