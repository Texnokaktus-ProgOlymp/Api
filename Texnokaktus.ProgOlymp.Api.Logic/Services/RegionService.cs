using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

internal class RegionService(AppDbContext context, IMemoryCache memoryCache) : IRegionService
{
    public async Task<Region[]> GetAllRegionsAsync() =>
        await memoryCache.GetOrCreateAsync("Regions:All",
                                           entry =>
                                           {
                                               entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
                                               return context.Regions
                                                             .OrderByDescending(region => region.Order)
                                                             .ThenBy(region => region.Id)
                                                             .Select(region => new Region(region.Id, region.Name))
                                                             .ToArrayAsync();
                                           })
     ?? [];

    public async Task<bool> ExistsAsync(int id) =>
        await memoryCache.GetOrCreateAsync($"Regions:{id}:Exists",
                                           entry =>
                                           {
                                               entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
                                               return context.Regions.AnyAsync(region => region.Id == id);
                                           });
}
