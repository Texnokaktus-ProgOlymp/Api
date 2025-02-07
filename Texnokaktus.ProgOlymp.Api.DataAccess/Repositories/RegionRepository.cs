using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Repositories;

public class RegionRepository(AppDbContext context) : IRegionRepository
{
    public Task<Region[]> GetAllAsync() =>
        context.Regions
               .AsNoTracking()
               .OrderByDescending(region => region.Order)
               .ThenBy(region => region.Id)
               .ToArrayAsync();

    public Task<bool> ExistsAsync(Expression<Func<Region, bool>> predicate) =>
        context.Regions
               .AsNoTracking()
               .AnyAsync(predicate);
}
