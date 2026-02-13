using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Context;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Api.Models;
using Texnokaktus.ProgOlymp.Api.Services.Abstractions;
using ParticipationState = Texnokaktus.ProgOlymp.Api.Domain.ParticipationState;

namespace Texnokaktus.ProgOlymp.Api.Services;

public class ParticipationService(AppDbContext context) : IParticipationService
{
    public async Task<Results<Ok<ContestParticipation>, NotFound>> GetParticipationAsync(int userId, string contestName)
    {
        var contest = await context.Contests
                                   .AsNoTracking()
                                   .AsSplitQuery()
                                   .Include(c => c.Applications.Where(a => a.UserId == userId))
                                   .ThenInclude(a => a.User)
                                   .FirstOrDefaultAsync(contest => contest.Name == contestName);

        if (contest is null) return TypedResults.NotFound();

        if (contest.Applications.FirstOrDefault() is not { } application)
            return TypedResults.Ok(new ContestParticipation
            {
                IsUserRegistered = false
            });

        return TypedResults.Ok(new ContestParticipation
        {
            IsUserRegistered = true,
            PreliminaryStageParticipation = application.PreliminaryStageParticipation?.MapContestStageParticipation(application.Contest.PreliminaryStage?.ContestId),
            FinalStageParticipation = application.FinalStageParticipation?.MapContestStageParticipation(null)
        });
    }
}

file static class MappingExtensions
{
    public static ContestStageParticipation MapContestStageParticipation(this Participation participation, long? contestId) =>
        new()
        {
            ContestId = contestId,
            Start = participation.Start,
            Finish = participation.Finish,
            State = participation.State.MapParticipationState()
        };

    private static ParticipationState MapParticipationState(this DataAccess.Entities.ParticipationState state) =>
        state switch
        {
            DataAccess.Entities.ParticipationState.NotStarted => ParticipationState.NotStarted,
            DataAccess.Entities.ParticipationState.InProgress => ParticipationState.InProgress,
            DataAccess.Entities.ParticipationState.Finished   => ParticipationState.Finished,
            _                                                 => throw new ArgumentOutOfRangeException(nameof(state), state, $"Unable to map {nameof(ParticipationState)}")
        };
}
