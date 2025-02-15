using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using Texnokaktus.ProgOlymp.Api.Converters;
using Texnokaktus.ProgOlymp.Api.DataAccess;
using Texnokaktus.ProgOlymp.Api.Endpoints;
using Texnokaktus.ProgOlymp.Api.Extensions;
using Texnokaktus.ProgOlymp.Api.Infrastructure;
using Texnokaktus.ProgOlymp.Api.Logic;
using Texnokaktus.ProgOlymp.Api.Settings;
using Texnokaktus.ProgOlymp.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb"))
                                                      .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()))
       .AddLogicServices()
       .AddPresentationServices();

builder.Services.AddOptions<JwtSettings>().BindConfiguration(nameof(JwtSettings));

var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("DefaultRedis")!);
builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
builder.Services.AddStackExchangeRedisCache(options => options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(connectionMultiplexer));

builder.Services.AddMemoryCache();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddGrpcClients(builder.Configuration);

builder.Services.AddOpenApi(options => options.AddSchemaTransformer<SchemaTransformer>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services
       .AddGrpcHealthChecks()
       .AddDatabaseHealthChecks();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddTexnokaktusOpenTelemetry(builder.Configuration,
                                             "API",
                                             null,
                                             meterProviderBuilder => meterProviderBuilder.AddMeter(Texnokaktus.ProgOlymp.Api.Logic.Observability.Constants.MeterName));

builder.Services
       .AddDataProtection(options => options.ApplicationDiscriminator = Assembly.GetEntryAssembly()?.GetName().Name)
       .PersistKeysToStackExchangeRedis(connectionMultiplexer);

builder.Services
       .AddAuthentication(options =>
        {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddConfiguredJwtBearer(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGrpcHealthChecksService();

// if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.ConfigObject.Urls = [new() { Name = "v1", Url = "/openapi/v1.json" }]);
    app.MapGrpcReflectionService();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api")
   .MapUsersEndpoints()
   .MapContestEndpoints()
   .MapRegionEndpoints();

/*
await using (var scope = app.Services.CreateAsyncScope())
{
       var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
       await context.Database.EnsureDeletedAsync();
       await context.Database.EnsureCreatedAsync();
       
       await using var f = File.OpenRead(@"D:\kav128\Downloads\regions.json");
       var jsonNode = JsonNode.Parse(f);
    
       context.Regions.AddRange(jsonNode.AsArray()
                                        .Select(x => new Region
                                         {
                                                Id = x["Id"].GetValue<int>(),
                                                Name = x["Name"].GetValue<string>(),
                                                Order = x["Id"].GetValue<int>() switch
                                                {
                                                       78 => 10,
                                                       47 => 9,
                                                       77 => 5,
                                                       _  => 0
                                                }
                                         }));
       
       await context.SaveChangesAsync();

       var contestService = scope.ServiceProvider.GetRequiredService<IContestService>();
       await contestService.AddContestAsync("Олимпиада по информатике и программированию 2025",
                                            new(2025, 02, 22, 00, 00, 00, TimeSpan.FromHours(3)),
                                            new(2025, 03, 14, 00, 00, 00, TimeSpan.FromHours(3)),
                                            71377L,
                                            74534L);
       
       await context.ContestStages
                    .Where(stage => stage.Id == 71377L)
                    .ExecuteUpdateAsync(x => x.SetProperty(stage => stage.ContestFinish,
                                                           _ => new(2025, 03, 14, 00, 00, 00, TimeSpan.FromHours(3))));
}
*/

await app.RunAsync();
