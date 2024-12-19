using Blazor.Client.Hubs;
using Blazor.UI.Components;
using Blazored.LocalStorage;
using Orleans.Configuration;

namespace Blazor.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddSignalR();

            builder.Services.AddBlazoredLocalStorage();
            builder.Host.UseOrleansClient((context, client) =>
            {
                var configuration = builder.Configuration;

                client.UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "default";
                    options.ServiceId = "OrleansChatService";
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
            app.MapHub<ChatHub>("/chathub");
            app.Run();
        }
    }
}
