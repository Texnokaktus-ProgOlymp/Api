using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.DataAccess.Models;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

public interface IContestRepository
{
    Task<Contest?> GetById(int id);
    Contest AddContest(ContestInsertModel insertModel);
    Task<bool> UpdateAsync(int id, Func<Contest, bool> updateAction);
}
