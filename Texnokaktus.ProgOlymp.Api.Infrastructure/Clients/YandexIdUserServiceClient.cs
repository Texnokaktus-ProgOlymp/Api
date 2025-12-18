using Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexId;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients;

public class YandexIdUserServiceClient(UserService.UserServiceClient client) : IYandexIdUserServiceClient
{
    public async Task<string> GetOAuthUrlAsync(string? urlRequest, CancellationToken cancellationToken)
    {
        var request = new GetOAuthUrlRequest
        {
            RedirectUrl = urlRequest
        };
        var response = await client.GetOAuthUrlAsync(request, cancellationToken: cancellationToken);
        return response.Result;
    }

    public async Task<User> AuthenticateUserAsync(string code, CancellationToken cancellationToken)
    {
        var request = new AuthenticateUserRequest
        {
            Code = code
        };
        var response = await client.AuthenticateUserAsync(request, cancellationToken: cancellationToken);
        return response.Result;
    }
}
