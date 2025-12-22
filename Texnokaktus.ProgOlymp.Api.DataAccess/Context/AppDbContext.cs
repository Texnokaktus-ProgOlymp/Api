using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Texnokaktus.ProgOlymp.Api.DataAccess.Converters;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Context;

public class AppDbContext(DbContextOptions options, IDataProtectionProvider dataProtectionProvider) : DbContext(options)
{
    public DbSet<Contest> Contests { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contest>(builder =>
        {
            builder.HasKey(contest => contest.Id);
            builder.Property(contest => contest.Id).UseIdentityColumn();

            builder.Property(stage => stage.Name)
                   .IsUnicode()
                   .HasMaxLength(100);

            builder.HasOne<ContestStage>(contest => contest.PreliminaryStage)
                   .WithOne()
                   .HasForeignKey<Contest>(contest => contest.PreliminaryStageId);

            builder.HasOne<ContestStage>(contest => contest.FinalStage)
                   .WithOne()
                   .HasForeignKey<Contest>(contest => contest.FinalStageId);
        });

        modelBuilder.Entity<ContestStage>(builder =>
        {
            builder.Property(stage => stage.Name)
                   .IsUnicode()
                   .HasMaxLength(100);

            builder.Property(stage => stage.Duration)
                   .HasConversion(timeSpan => timeSpan.Ticks, ticks => TimeSpan.FromTicks(ticks));

            builder.HasMany(contestResult => contestResult.Problems)
                   .WithOne()
                   .HasForeignKey(problem => problem.ContestResultId);
        });

        modelBuilder.Entity<Region>(builder =>
        {
            builder.HasKey(region => region.Id);

            builder.Property(region => region.Id)
                   .ValueGeneratedNever();

            builder.Property(region => region.Name)
                   .IsUnicode()
                   .HasMaxLength(40);

            builder.Property(region => region.Order)
                   .HasDefaultValue(0);

            builder.HasIndex(region => region.Name)
                   .IsUnique();
        });

        modelBuilder.Entity<Application>(builder =>
        {
            builder.HasKey(application => application.Id);

            builder.HasAlternateKey(application => new { application.ContestId, application.UserId });

            builder.Property(application => application.Snils)
                   .IsEncrypted(dataProtectionProvider.CreateProtector(nameof(Application.Snils)));

            builder.HasOne(application => application.User)
                   .WithMany()
                   .HasForeignKey(application => application.UserId);

            builder.HasOne(application => application.Contest)
                   .WithMany(contest => contest.Applications)
                   .HasForeignKey(application => application.ContestId);

            builder.HasOne(application => application.Region)
                   .WithMany()
                   .HasForeignKey(application => application.RegionId);

            builder.OwnsOne<ThirdPerson>(application => application.Parent);
            builder.OwnsOne<Teacher>(application => application.Teacher);

            builder.OwnsOne<Participation>(application => application.PreliminaryStageParticipation);
            builder.OwnsOne<Participation>(application => application.FinalStageParticipation);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(user => user.Id);

            builder.HasAlternateKey(user => user.Login);

            builder.Property(user => user.Login).IsUnicode(false) /*.HasMaxLength(128)*/;
            builder.Property(user => user.DisplayName).IsUnicode() /*.HasMaxLength(128)*/;
            builder.Property(user => user.DefaultAvatar).IsUnicode(false) /*.HasMaxLength(96)*/;
        });

        modelBuilder.Entity<Problem>(builder =>
        {
            builder.HasKey(problem => problem.Id);

            builder.HasAlternateKey(problem => new { problem.ContestResultId, problem.Alias });

            builder.HasMany(problem => problem.Results)
                   .WithOne()
                   .HasForeignKey(problemResult => problemResult.ProblemId);
        });

        modelBuilder.Entity<ProblemResult>(builder =>
        {
            builder.HasKey(problemResult => problemResult.Id);

            builder.HasAlternateKey(problemResult => new { problemResult.ProblemId, problemResult.ApplicationId });

            builder.Property(problemResult => problemResult.BaseScore).HasScorePrecision();

            builder.HasOne(problemResult => problemResult.Application)
                   .WithMany()
                   .HasForeignKey(problemResult => problemResult.ApplicationId);

            builder.OwnsMany<ScoreAdjustment>(problemResult => problemResult.Adjustments,
                                              navigationBuilder =>
                                              {
                                                  navigationBuilder.HasKey(adjustment => adjustment.Id);

                                                  navigationBuilder.Property(adjustment => adjustment.Adjustment)
                                                                   .HasScorePrecision();
                                              });
        });

        base.OnModelCreating(modelBuilder);
    }
}

file static class EfExtensions
{
    public static PropertyBuilder<string?> IsEncrypted(this PropertyBuilder<string?> propertyBuilder,
                                                       IDataProtector dataProtector) =>
        propertyBuilder.HasConversion(new EncryptionConverter(dataProtector));

    public static PropertyBuilder<decimal> HasScorePrecision(this PropertyBuilder<decimal> propertyBuilder) =>
        propertyBuilder.HasPrecision(5, 2);
}
