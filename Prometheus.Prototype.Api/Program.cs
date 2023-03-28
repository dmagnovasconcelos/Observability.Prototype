using Prometheus.Prototype.Api;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
      .Enrich
        .FromLogContext()
      .Enrich
        .WithMachineName()
      .WriteTo
        .Console()
      .WriteTo
        .Prometheus("events_{0}", "{0}")
      .WriteTo
        .Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfigurarion:Uri"]))
        {
            IndexFormat = $"prometheusprototype-logs-{DateTime.UtcNow:yyyy-MM}",
            AutoRegisterTemplate = true,
            NumberOfShards = 2,
            NumberOfReplicas = 1
        })
      .Enrich
        .WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
      .ReadFrom
        .Configuration(context.Configuration);
});


var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, builder.Environment);
app.Run();