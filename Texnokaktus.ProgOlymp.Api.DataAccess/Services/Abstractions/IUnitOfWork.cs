using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Services.Abstractions;

public interface IUnitOfWork
{
    IContestRepository ContestRepository { get; }
    IUserRepository UserRepository { get; }
    Task<int> SaveChangesAsync();
}
