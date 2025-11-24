using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.Api.DataAccess.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Services;

internal class UnitOfWork(AppDbContext context,
                          IApplicationRepository applicationRepository,
                          IContestRepository contestRepository,
                          IUserRepository userRepository) : IUnitOfWork
{
    public IApplicationRepository ApplicationRepository => applicationRepository;
    public IContestRepository ContestRepository => contestRepository;
    public IUserRepository UserRepository => userRepository;
    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
