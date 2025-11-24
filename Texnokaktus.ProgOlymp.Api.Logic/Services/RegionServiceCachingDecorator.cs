using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

internal class RegionServiceCachingDecorator(IRegionService regionService, IMemoryCache memoryCache) : IRegionService
{
    public async Task<Region[]> GetAllRegionsAsync() =>
        await memoryCache.GetOrCreateAsync("Regions:All",
                                           entry =>
                                           {
                                               entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
                                               return regionService.GetAllRegionsAsync();
                                           })
     ?? [];

    public async Task<bool> ExistsAsync(int id) =>
        await memoryCache.GetOrCreateAsync($"Regions:{id}:Exists",
                                           entry =>
                                           {
                                               entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
                                               return regionService.ExistsAsync(id);
                                           });
}
