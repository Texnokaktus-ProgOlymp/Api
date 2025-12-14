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
using Texnokaktus.ProgOlymp.Api.Services.Grpc;
using Texnokaktus.ProgOlymp.Api.Settings;
using Texnokaktus.ProgOlymp.Api.Validators;
using Texnokaktus.ProgOlymp.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb"))
                                                      .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()))
       .AddLogicServices()
       .AddPresentationServices()
       .AddValidators();

builder.Services.AddOptions<JwtSettings>().BindConfiguration(nameof(JwtSettings));

var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("DefaultRedis")!);
builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
builder.Services.AddStackExchangeRedisCache(options => options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(connectionMultiplexer));

builder.Services.AddMemoryCache();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddGrpcClients(builder.Configuration);

builder.Services.AddOpenApi(options => options.AddSchemaTransformer<SchemaTransformer>());

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services
       .AddGrpcHealthChecks()
       .AddDatabaseHealthChecks();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddTexnokaktusOpenTelemetry("API",
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

builder.Services.AddCors(options =>
{ 
    options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin()
                                                           .AllowAnyHeader()
                                                           .AllowAnyMethod());
});

var app = builder.Build();

app.MapGrpcHealthChecksService();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.ConfigObject.Urls = [new() { Name = "v1", Url = "/openapi/v1.json" }]);
    app.MapGrpcReflectionService();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<ParticipantServiceImpl>();
app.MapGrpcService<RegistrationDataServiceImpl>();

app.MapGroup("api")
   .MapUsersEndpoints()
   .MapContestEndpoints()
   .MapRegionEndpoints();

await app.RunAsync();
