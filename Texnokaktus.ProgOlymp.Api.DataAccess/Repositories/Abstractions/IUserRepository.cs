using System.Linq.Expressions;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.DataAccess.Models;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

public interface IUserRepository
{
    Task<User[]> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByLoginAsync(string login);
    User AddUser(UserInsertModel insertModel);
    Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate);
}
