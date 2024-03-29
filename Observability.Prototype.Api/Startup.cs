﻿using Prometheus.DotNetRuntime;

namespace Prometheus.Prototype.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static IDisposable? Collector;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Collector = CreateCollector();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseHttpMetrics();
            app.UseMetricServer();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });
        }

        public static IDisposable CreateCollector()
        {
            var builder = DotNetRuntimeStatsBuilder.Customize()
                .WithContentionStats(CaptureLevel.Informational)
                .WithGcStats(CaptureLevel.Verbose)
                .WithThreadPoolStats(CaptureLevel.Informational)
                .WithExceptionStats(CaptureLevel.Errors)
                .WithJitStats(CaptureLevel.Informational)
                .WithSocketStats();

            builder.RecycleCollectorsEvery(new TimeSpan(0, 20, 0));

            return builder.StartCollecting();
        }
    }
}
