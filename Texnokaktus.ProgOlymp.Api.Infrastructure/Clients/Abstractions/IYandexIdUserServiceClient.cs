using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexId;

namespace Texnokaktus.ProgOlymp.Api.Infrastructure.Clients.Abstractions;

public interface IYandexIdUserServiceClient
{
    Task<string> GetOAuthUrlAsync(string? urlRequest, CancellationToken cancellationToken);
    Task<User> AuthenticateUserAsync(string code, CancellationToken cancellationToken);
}
