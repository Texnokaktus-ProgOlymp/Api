using Texnokaktus.ProgOlymp.Api.Models;

namespace Texnokaktus.ProgOlymp.Api.Services.Abstractions;

public interface IRegionService
{
    Task<Region[]> GetAllRegionsAsync();
    Task<bool> ExistsAsync(int id);
}
