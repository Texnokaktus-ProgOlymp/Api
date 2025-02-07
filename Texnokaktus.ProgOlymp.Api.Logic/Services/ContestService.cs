using Texnokaktus.ProgOlymp.Api.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

internal class ContestService(IUnitOfWork unitOfWork, IContestDataServiceClient contestDataServiceClient) : IContestService
{
    public async Task<int> AddContestAsync(string name,
                                           DateTimeOffset registrationStart,
                                           DateTimeOffset registrationFinish,
                                           long? preliminaryStageId,
                                           long? finalStageId)
    {
        var contest = unitOfWork.ContestRepository.AddContest(new(name, registrationStart, registrationFinish));

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

        await unitOfWork.SaveChangesAsync();

        return contest.Id;
    }

    public async Task<Contest?> GetContestAsync(int id)
    {
        var contest = await unitOfWork.ContestRepository.GetById(id);
        return contest?.MapContest();
    }
}

file static class MappingExtensions
{
    public static Contest MapContest(this DataAccess.Entities.Contest contest) =>
        new(contest.Id,
            contest.Name,
            contest.RegistrationStart,
            contest.RegistrationFinish,
            contest.PreliminaryStage?.MapContestStage(),
            contest.FinalStage?.MapContestStage());

    private static ContestStage MapContestStage(this DataAccess.Entities.ContestStage contestStage) =>
        new(contestStage.Id,
            contestStage.Name,
            contestStage.ContestStart,
            contestStage.ContestFinish,
            contestStage.Duration);
}
