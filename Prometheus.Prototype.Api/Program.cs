using Prometheus.Prototype.Api;
using Serilog;
using Serilog.Sinks.Elasticsearch;

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
        .Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfigurarion:Uri"]))
        {
            IndexFormat = $"{context.Configuration["ApplicationName"]}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
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