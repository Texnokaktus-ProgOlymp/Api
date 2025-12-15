using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ParticipationService(AppDbContext context) : IParticipationService
{
    public async Task<ContestParticipation> GetContestParticipationAsync(int userId, string contestName)
    {
        var app = await context.Applications
                               .AsSplitQuery()
                               .AsNoTracking()
                               .Include(application => application.User)
                               .Include(application => application.Contest)
                               .FirstOrDefaultAsync(application => application.Contest.Name == contestName
                                                                && application.UserId == userId);

        if (app is null)
            return new(false, null, null);

        return new(true,
                   app.PreliminaryStageParticipation?.MapContestStageParticipation(app.Contest.PreliminaryStage?.ContestId),
                   app.FinalStageParticipation?.MapContestStageParticipation(null));
    }
}

file static class MappingExtensions
{
    public static ContestStageParticipation MapContestStageParticipation(this DataAccess.Entities.Participation participation, long? contestId) =>
        new(contestId,
            participation.Start,
            participation.Finish,
            participation.State.MapParticipationState());

    private static ParticipationState MapParticipationState(this DataAccess.Entities.ParticipationState state) =>
        state switch
        {
            DataAccess.Entities.ParticipationState.NotStarted => ParticipationState.NotStarted,
            DataAccess.Entities.ParticipationState.InProgress => ParticipationState.InProgress,
            DataAccess.Entities.ParticipationState.Finished   => ParticipationState.Finished,
            _                                                 => throw new ArgumentOutOfRangeException(nameof(state), state, $"Unable to map {nameof(ParticipationState)}")
        };
}
