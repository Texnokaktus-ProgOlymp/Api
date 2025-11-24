using Texnokaktus.ProgOlymp.Api.Domain;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

public interface IRegionService
{
    Task<Region[]> GetAllRegionsAsync();
    Task<bool> ExistsAsync(int id);
}
