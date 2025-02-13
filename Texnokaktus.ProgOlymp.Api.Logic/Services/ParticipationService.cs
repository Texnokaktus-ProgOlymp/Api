using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Api.Logic.Exceptions;
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

        var user = await userService.GetByIdAsync(userId) ?? throw new UserNotFoundException(userId);
        var contest = await contestService.GetContestAsync(contestId) ?? throw new ContestNotFoundException(contestId);

        var preliminaryParticipation = await GetContestStageParticipation(user, contest.PreliminaryStage);

        return new(true, preliminaryParticipation, null);
    }

    private async Task<ContestStageParticipation?> GetContestStageParticipation(User user, ContestStage? contestStage)
    {
        if (contestStage is null)
            return null;

        var participantStatus = await contestDataServiceClient.GetParticipantStatusAsync(contestStage.Id, user.Login);

        var state = participantStatus.State.MapParticipationState();

        if (state is ParticipationState.InProgress or ParticipationState.Finished)
            return new(contestStage.ContestStart,
                       contestStage.ContestFinish,
                       state,
                       await resultService.GetContestResultsAsync(user.Login, contestStage.Id));

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
