using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Api.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Context;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Contest> Contests { get; set; }
    public DbSet<ContestStage> ContestStages { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Application> Applications { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contest>(builder =>
        {
            builder.HasKey(contest => contest.Id);
            builder.Property(contest => contest.Id).UseIdentityColumn();

            builder.HasOne<ContestStage>(contest => contest.PreliminaryStage)
                   .WithOne()
                   .HasForeignKey<Contest>(contest => contest.PreliminaryStageId);

            builder.HasOne<ContestStage>(contest => contest.FinalStage)
                   .WithOne()
                   .HasForeignKey<Contest>(contest => contest.FinalStageId);
        });

        modelBuilder.Entity<ContestStage>(builder =>
        {
            builder.HasKey(stage => stage.Id);
            builder.Property(stage => stage.Id).ValueGeneratedNever();

            builder.Property(stage => stage.Duration)
                   .HasConversion(timeSpan => timeSpan.Ticks, ticks => TimeSpan.FromTicks(ticks));
        });
        
        modelBuilder.Entity<Region>(builder =>
        {
            builder.HasKey(region => region.Id);
            builder.Property(region => region.Id)
                   .ValueGeneratedNever();

            builder.Property(region => region.Order)
                   .HasDefaultValue(0);

            builder.HasIndex(region => region.Name)
                   .IsUnique();
        });

        modelBuilder.Entity<Application>(builder =>
        {
            builder.HasKey(application => application.Id);

            builder.HasAlternateKey(application => new { application.ContestId, application.YandexIdLogin });

            builder.HasOne(application => application.Region)
                   .WithMany()
                   .HasForeignKey(application => application.RegionId);

            builder.HasOne(application => application.Contest)
                   .WithMany()
                   .HasForeignKey(application => application.ContestId);

            builder.OwnsOne<ThirdPerson>(application => application.Parent);
            builder.OwnsOne<Teacher>(application => application.Teacher);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}
