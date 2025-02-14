using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients;

internal class ContestDataServiceClient(ContestDataService.ContestDataServiceClient client) : IContestDataServiceClient
{
    public async Task<string?> GetContestUrlAsync(long contestId)
    {
        var request = new GetContestUrlRequest
        {
            ContestId = contestId
        };

        var response = await client.GetContestUrlAsync(request);
        return response.ContestUrl;
    }

    public async Task<ContestDescription> GetContestAsync(long contestId)
    {
        var request = new GetContestRequest
        {
            ContestId = contestId
        };

        var response = await client.GetContestAsync(request);
        return response.Result;
    }

    public async Task<IEnumerable<ContestProblem>> GetContestProblemsAsync(long contestId)
    {
        var request = new GetProblemsRequest
        {
            ContestId = contestId
        };

        var response = await client.GetProblemsAsync(request);
        return response.Problems;
    }

    public async Task<ContestStandings> GetStandingsAsync(long contestStageId, int pageIndex, int pageSize, string? participantSearch)
    {
        var request = new GetStandingsRequest
        {
            ContestId = contestStageId,
            PageIndex = pageIndex,
            PageSize = pageSize,
            ParticipantSearch = participantSearch
        };

        var response = await client.GetStandingsAsync(request);
        return response.Result;
    }

    public async Task<ParticipantStatus> GetParticipantStatusAsync(long contestStageId, string participantLogin)
    {
        var request = new ParticipantStatusRequest
        {
            ContestId = contestStageId,
            ParticipantLogin = participantLogin
        };

        var response = await client.GetParticipantStatusAsync(request);
        return response.Result;
    }

    public async Task<ParticipantStats> GetParticipantStatsAsync(long contestStageId, string participantLogin)
    {
        var request = new ParticipantStatsRequest
        {
            ContestId = contestStageId,
            ParticipantLogin = participantLogin
        };

        var response = await client.GetParticipantStatsAsync(request);
        return response.Result;
    }
}
