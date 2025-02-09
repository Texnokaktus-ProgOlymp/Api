using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexId;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;

public class YandexIdUserServiceClient(UserService.UserServiceClient client) : IYandexIdUserServiceClient
{
    public async Task<string> GetOAuthUrlAsync(string? urlRequest)
    {
        var request = new GetOAuthUrlRequest
        {
            RedirectUrl = urlRequest
        };
        var response = await client.GetOAuthUrlAsync(request);
        return response.Result;
    }

    public async Task<User> AuthenticateUserAsync(string code)
    {
        var request = new AuthenticateUserRequest
        {
            Code = code
        };
        var response = await client.AuthenticateUserAsync(request);
        return response.Result;
    }
}
