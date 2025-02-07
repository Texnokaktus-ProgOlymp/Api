﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories;
using Texnokaktus.ProgOlymp.Api.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.Api.DataAccess.Services;
using Texnokaktus.ProgOlymp.Api.DataAccess.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Api.DataAccess;

public static class DiUtils
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection,
                                                   Action<DbContextOptionsBuilder> optionsAction) =>
        serviceCollection.AddDbContext<AppDbContext>(optionsAction)
                         .AddScoped<IUnitOfWork, UnitOfWork>()
                         .AddScoped<IRegionRepository, RegionRepository>()
                         .AddScoped<IApplicationRepository, ApplicationRepository>()
                         .AddScoped<IContestRepository, ContestRepository>();

    public static IHealthChecksBuilder AddDatabaseHealthChecks(this IHealthChecksBuilder builder) =>
        builder.AddDbContextCheck<AppDbContext>("database");
}
