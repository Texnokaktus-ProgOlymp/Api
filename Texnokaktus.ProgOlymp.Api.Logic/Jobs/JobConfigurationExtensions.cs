using Quartz;

namespace Texnokaktus.ProgOlymp.Api.Logic.Jobs;

public static class JobConfigurationExtensions
{
    extension(IServiceCollectionQuartzConfigurator configurator)
    {
        public IServiceCollectionQuartzConfigurator AddParticipationUpdateJob(Action<IJobConfigurator>? jobConfiguratorAction = null, Action<ITriggerConfigurator>? triggerConfiguratorAction = null)
        {
            const string jobName = nameof(ParticipationUpdateJob);

            configurator.AddJob<ParticipationUpdateJob>(jobConfigurator => jobConfigurator.WithIdentity(jobName)
                                                                                          .DisallowConcurrentExecution()
                                                                                          .Apply(jobConfiguratorAction));

            configurator.AddTrigger(triggerConfigurator => triggerConfigurator.ForJob(jobName)
                                                                              .WithIdentity($"{jobName}-trigger")
                                                                              .Apply(triggerConfiguratorAction));

            return configurator;
        }
    }

    private static TBuilder Apply<TBuilder>(this TBuilder builder, Action<TBuilder>? action)
    {
        action?.Invoke(builder);
        return builder;
    }
}
