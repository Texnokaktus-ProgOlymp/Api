using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

internal class ContestService(AppDbContext context, IContestDataServiceClient contestDataServiceClient) : IContestService
{
    public async Task<int> AddContestAsync(string name,
                                           DateTimeOffset registrationStart,
                                           DateTimeOffset registrationFinish,
                                           long? preliminaryStageId,
                                           long? finalStageId)
    {
        var contest = new Contest
        {
            Name = name,
            RegistrationStart = registrationStart,
            RegistrationFinish = registrationFinish
        };

        if (preliminaryStageId.HasValue)
        {
            var description = await contestDataServiceClient.GetContestAsync(preliminaryStageId.Value);
            contest.PreliminaryStage = new()
            {
                Id = preliminaryStageId.Value,
                Name = description.Name,
                ContestStart = description.StartTime.ToDateTimeOffset(),
                Duration = description.Duration.ToTimeSpan()
            };
        }

        if (finalStageId.HasValue)
        {
            var description = await contestDataServiceClient.GetContestAsync(finalStageId.Value);
            contest.FinalStage = new()
            {
                Id = finalStageId.Value,
                Name = description.Name,
                ContestStart = description.StartTime.ToDateTimeOffset(),
                Duration = description.Duration.ToTimeSpan()
            };
        }

        await context.SaveChangesAsync();

        return contest.Id;
    }

    public Task<Domain.Contest?> GetContestAsync(string contestName) =>
        context.Contests
               .Include(contest => contest.PreliminaryStage)
               .Include(contest => contest.FinalStage)
               .Select(contest => contest.MapContest())
               .FirstOrDefaultAsync(contest => contest.Name == contestName);
}

file static class MappingExtensions
{
    public static Domain.Contest MapContest(this Contest contest) =>
        new(contest.Id,
            contest.Name,
            contest.RegistrationStart,
            contest.RegistrationFinish,
            contest.PreliminaryStage?.MapContestStage(),
            contest.FinalStage?.MapContestStage());

    private static Domain.ContestStage MapContestStage(this ContestStage contestStage) =>
        new(contestStage.Id,
            contestStage.Name,
            contestStage.ContestStart,
            contestStage.ContestFinish,
            contestStage.Duration);
}
