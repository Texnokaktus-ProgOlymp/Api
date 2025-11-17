using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.DataAccess.Models;
using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Repositories;

public class ContestRepository(AppDbContext context) : IContestRepository
{
    public Task<Contest?> GetById(int id) =>
        context.Contests
               .AsNoTracking()
               .Include(contest => contest.PreliminaryStage)
               .Include(contest => contest.FinalStage)
               .FirstOrDefaultAsync(contest => contest.Id == id);

    public Task<Contest?> GetByName(string name) =>
        context.Contests
               .AsNoTracking()
               .Include(contest => contest.PreliminaryStage)
               .Include(contest => contest.FinalStage)
               .FirstOrDefaultAsync(contest => contest.Name == name);

    public Contest AddContest(ContestInsertModel insertModel)
    {
        var entity = new Contest
        {
            Name = insertModel.Name,
            RegistrationStart = insertModel.RegistrationStart,
            RegistrationFinish = insertModel.RegistrationFinish
        };

        return context.Contests.Add(entity).Entity;
    }

    public async Task<bool> UpdateAsync(int id, Func<Contest, bool> updateAction)
    {
        var entity = await context.Contests
                                  .Include(contest => contest.PreliminaryStage)
                                  .Include(contest => contest.FinalStage)
                                  .FirstOrDefaultAsync(contest => contest.Id == id);

        if (entity is null)
            return false;

        if (!updateAction.Invoke(entity))
            return false;

        await context.SaveChangesAsync();
        return true;
    }
}
