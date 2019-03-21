using System.Threading.Tasks;
using RabbitMqPingPong.Config;
using RabbitMqPingPong.Migration;
using RabbitMqPingPong.Mqtt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using uPLibrary.Networking.M2Mqtt;

namespace RabbitMqPingPong.Service
{
    public class Startup
    {
        private ILogger<Startup> StartupLogger;
        
        public static readonly IConfiguration Configuration = ConfigBuilderExtensions.GetConfiguration();
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .RegisterService(Configuration)
                .AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            IApplicationLifetime lifetime,
            ILoggerFactory loggerFactory, 
            IBus bus,
            MqttClient mqttClient)
        {
            StartupLogger = loggerFactory.CreateLogger<Startup>();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            StartupLogger.LogInformation("Begin Application_Start");
            lifetime.ApplicationStarted.Register(() => StartupLogger.LogInformation("End Application_Start"));
            lifetime.ApplicationStopping.Register(() => StartupLogger.LogInformation("Start Application_Stop"));
            lifetime.ApplicationStopped.Register(() => StartupLogger.LogInformation("End Application_Stop"));
            
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            if (configuration.GetValue("host:migrateDbSchema", true))
            {
                app.ApplicationServices.MigrateUpSchema();
                StartupLogger.LogInformation("Finished database migration");
            }

            await SubscriptionRegistration.SetupSubscriptions(bus);
            mqttClient.Setup(app.ApplicationServices);
            
            app.UseHealthChecks("/status/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                ResponseWriter = HealthCheckResponseWriter
            });
            app.UseMvc();
        }
        
        private static Task HealthCheckResponseWriter(HttpContext context, HealthReport result)
        {
            var output = $"Overall result: {result.Status}\n--------------------------\n";
            foreach (var item in result.Entries)
            {
                output += $"{item.Key}: {item.Value.Status} => {item.Value.Description}\n";
            }
            return context.Response.WriteAsync(output);
        }
    }
}