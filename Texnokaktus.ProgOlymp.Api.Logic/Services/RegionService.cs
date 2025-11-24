using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.Domain;
using Texnokaktus.ProgOlymp.Api.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.Logic.Services;

internal class RegionService(AppDbContext context) : IRegionService
{
    public Task<Region[]> GetAllRegionsAsync() =>
        context.Regions
               .OrderByDescending(region => region.Order)
               .ThenBy(region => region.Id)
               .Select(region => new Region(region.Id, region.Name))
               .ToArrayAsync();

    public Task<bool> ExistsAsync(int id) => context.Regions.AnyAsync(region => region.Id == id);
}
