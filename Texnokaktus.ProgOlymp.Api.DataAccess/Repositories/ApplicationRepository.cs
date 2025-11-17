using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.DataAccess.Models;
using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Repositories;

public class ApplicationRepository(AppDbContext context) : IApplicationRepository
{
    public Task<Application[]> GetApplicationsAsync(string contestName) =>
        context.Applications
               .AsNoTracking()
               .Include(application => application.User)
               .Include(application => application.Region)
               .Where(application => application.Contest.Name == contestName)
               .ToArrayAsync();

    public Application Add(ApplicationInsertModel insertModel)
    {
        var entity = new Application
        {
            UserId = insertModel.UserId,
            ContestId = insertModel.ContestId,
            Uid = insertModel.Uid,
            Created = insertModel.Created,
            FirstName = insertModel.Name.FirstName,
            LastName = insertModel.Name.LastName,
            Patronym = insertModel.Name.Patronym,
            BirthDate = insertModel.BirthDate,
            Snils = insertModel.Snils,
            Email = insertModel.Email,
            SchoolName = insertModel.SchoolName,
            RegionId = insertModel.RegionId,
            Parent = new()
            {
                FirstName = insertModel.Parent.Name.FirstName,
                LastName = insertModel.Parent.Name.LastName,
                Patronym = insertModel.Parent.Name.Patronym,
                Email = insertModel.Parent.Email,
                Phone = insertModel.Parent.Phone
            },
            Teacher = new()
            {
                School = insertModel.Teacher.School,
                FirstName = insertModel.Teacher.Name.FirstName,
                LastName = insertModel.Teacher.Name.LastName,
                Patronym = insertModel.Teacher.Name.Patronym,
                Email = insertModel.Teacher.Email,
                Phone = insertModel.Teacher.Phone
            },
            PersonalDataConsent = insertModel.PersonalDataConsent,
            Grade = insertModel.Grade
        };

        return context.Applications
                      .Add(entity)
                      .Entity;
    }

    public Task<bool> ExistsAsync(Expression<Func<Application, bool>> predicate) =>
        context.Applications
               .AsNoTracking()
               .AnyAsync(predicate);
}
