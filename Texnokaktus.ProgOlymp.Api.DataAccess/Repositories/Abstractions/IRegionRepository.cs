using System.Linq.Expressions;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

public interface IRegionRepository
{
    Task<Region[]> GetAllAsync();
    Task<bool> ExistsAsync(Expression<Func<Region, bool>> predicate);
}
