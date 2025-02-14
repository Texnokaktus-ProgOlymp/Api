using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

public class ParticipationService(IUserService userService,
                                  IRegistrationService registrationService,
                                  IContestService contestService,
                                  IContestDataServiceClient contestDataServiceClient,
                                  IResultService resultService) : IParticipationService
{
    public async Task<ContestParticipation> GetContestParticipationAsync(int userId, int contestId)
    {
        if (!await registrationService.IsUserRegisteredAsync(contestId, userId))
            return new(false, null, null);

        var user = await userService.GetRequiredUserAsync(userId);
        var contest = await contestService.GetRequiredContestAsync(contestId);

        var preliminaryParticipation = await GetContestStageParticipationAsync(user.Login, contest.PreliminaryStage);

        return new(true, preliminaryParticipation, null);
    }

    private async Task<ContestStageParticipation?> GetContestStageParticipationAsync(string userLogin, ContestStage? contestStage)
    {
        if (contestStage is null)
            return null;

        var participantStatus = await contestDataServiceClient.GetParticipantStatusAsync(contestStage.Id, userLogin);

        var state = participantStatus.State.MapParticipationState();

        if (state is ParticipationState.InProgress or ParticipationState.Finished)
            return new(contestStage.ContestStart,
                       contestStage.ContestFinish,
                       state,
                       await resultService.GetContestResultsAsync(userLogin, contestStage.Id));

        return new(contestStage.ContestStart, contestStage.ContestFinish, state, null);
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
