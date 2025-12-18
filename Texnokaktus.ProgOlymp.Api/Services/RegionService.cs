using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.Models;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Services;

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
                                                             .Select(region => new Region
                                                              {
                                                                  Id = region.Id,
                                                                  Name = region.Name
                                                              })
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
