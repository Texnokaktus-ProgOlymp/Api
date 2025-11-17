using System.Linq.Expressions;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.DataAccess.Models;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

public interface IApplicationRepository
{
    Task<Application[]> GetApplicationsAsync(string contestName);
    Application Add(ApplicationInsertModel insertModel);
    Task<bool> ExistsAsync(Expression<Func<Application, bool>> predicate);
}
