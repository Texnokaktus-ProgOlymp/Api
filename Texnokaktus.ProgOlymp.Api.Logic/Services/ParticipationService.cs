using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ParticipationService(IParticipantServiceClient participantServiceClient,
                                  IResultService resultService,
                                  AppDbContext context) : IParticipationService
{
    public async Task<ContestParticipation> GetContestParticipationAsync(int userId, string contestName)
    {
        var app = await context.Applications
                               .AsSplitQuery()
                               .AsNoTracking()
                               .Include(application => application.User)
                               .Include(application => application.Contest)
                               .ThenInclude(contest => contest.PreliminaryStage)
                               .Include(application => application.Contest)
                               .ThenInclude(contest => contest.FinalStage)
                               .FirstOrDefaultAsync(application => application.Contest.Name == contestName
                                                                && application.UserId == userId);

        if (app is null)
            return new(false, null, null);

        var preliminaryParticipation = await GetContestStageParticipationAsync(app.Id, app.Contest.PreliminaryStage);

        return new(true, preliminaryParticipation, null);
    }

    private async Task<ContestStageParticipation?> GetContestStageParticipationAsync(int participantId, DataAccess.Entities.ContestStage? contestStage)
    {
        if (contestStage is null)
            return null;

        var participantStatus = await participantServiceClient.GetParticipantStatusAsync(contestStage.Id, participantId);

        var state = participantStatus.State.MapParticipationState();

        if (state is ParticipationState.InProgress or ParticipationState.Finished)
            return new(contestStage.Id,
                       contestStage.ContestStart,
                       contestStage.ContestFinish,
                       state,
                       await resultService.GetContestResultsAsync(contestStage.Id, participantId));

        return new(contestStage.Id, contestStage.ContestStart, contestStage.ContestFinish, state, null);
    }
}

file static class MappingExtensions
{
    public static ParticipationState MapParticipationState(this Common.Contracts.Grpc.YandexContest.ParticipationState state) =>
        state switch
        {
            Common.Contracts.Grpc.YandexContest.ParticipationState.NotStarted => ParticipationState.NotStarted,
            Common.Contracts.Grpc.YandexContest.ParticipationState.InProgress => ParticipationState.InProgress,
            Common.Contracts.Grpc.YandexContest.ParticipationState.Finished   => ParticipationState.Finished,
            _                                                                 => throw new ArgumentOutOfRangeException(nameof(state), state, $"Unable to map {nameof(ParticipationState)}")
        };
}
