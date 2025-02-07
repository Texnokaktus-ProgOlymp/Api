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
}
