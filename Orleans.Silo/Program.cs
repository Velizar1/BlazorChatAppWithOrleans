using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Silo.Hubs;
using System.Net;

namespace Orleans.Silo
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Host
            .UseOrleans(siloBuilder =>
            {
                siloBuilder.AddAdoNetGrainStorage("Default", options =>
                    {
                        options.Invariant = "System.Data.SqlClient";
                        options.ConnectionString = "Server=.;Database=SiloChat;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true;";
                    });

                siloBuilder.UseLocalhostClustering()
                     .Configure<EndpointOptions>(options =>
                     {
                         options.SiloPort = 11111;
                         options.GatewayPort = 30000;
                     });

                siloBuilder.UseDashboard();
            });

            builder.Services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
                o.KeepAliveInterval = TimeSpan.FromSeconds(15);

            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("https://localhost:7048")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5001, listenOptions =>
                {
                    listenOptions.UseHttps();  // Ensure WebSockets are enabled
                });
            });

            var app = builder.Build();
            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigin");

            app.MapHub<ChatHub>("/chathub");

            await app.RunAsync();

            Console.WriteLine("Silo server has started");
            Console.ReadLine();

            await app.StopAsync();
        }
    }
}
