using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IRegionService
{
    Task<IEnumerable<Region>> GetAllRegionsAsync();
    Task<bool> ExistsAsync(int id);
}
