using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.Logging;


using  ReleaseRetation.Models;
using ReleaseRetation.Controllers;
using ReleaseRetation;
using System.IO;
using Serilog.Events;

namespace ReleaseRetation.Test
{
    public class SwaggerIntegration
    {
        private bool _started;
        public IHost ApiHost { get; set; }

        public HttpClient HttpTestClient;
        private IHostBuilder hostBuilder;

     
        public SwaggerIntegration()
        {
            //Create a logger
            var logger = new LoggerConfiguration().WriteTo.RollingFile(
                restrictedToMinimumLevel: LogEventLevel.Information,
                pathFormat: Path.Combine(Directory.GetCurrentDirectory(), "Logs/Log-{Date}.text"))
                .CreateLogger();

            //Create host
            hostBuilder = new HostBuilder()
               .ConfigureWebHost(webHost =>
               {
                   // Add TestServer
                   webHost.UseTestServer();
                   webHost.UseEnvironment("Test");
                   //Use standard Startup
                   webHost.UseStartup<ReleaseRetation.Startup>();
                   //Setup the Config
                   webHost.ConfigureAppConfiguration((hostingContext, config) =>
                   {
                       config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                       config.AddEnvironmentVariables();
                   });

               })
               .ConfigureLogging((context, builder) =>
                   {
                       builder.AddConsole();
                       //builder.AddFile();
                       builder.AddSerilog(logger);
                  
                   });
               
        }

        public async Task<SwaggerIntegration> Start()
        {
            ApiHost = await hostBuilder.StartAsync();

            // Create an HttpClient which is setup for the test host
            HttpTestClient = ApiHost.GetTestClient();

            _started = true;
            Log.Logger.Information("Integration Test Started");

            return this;
        }


        public void Stop()
        {
            if (!_started) return;
            _started = false;

            HttpTestClient.CancelPendingRequests();
            HttpTestClient.Dispose();
            HttpTestClient = null;

            Log.Logger.Information("Integration Test Stopped");
        }

        public void Dispose()
        {
            if (!_started) return;
        }
    }
}
